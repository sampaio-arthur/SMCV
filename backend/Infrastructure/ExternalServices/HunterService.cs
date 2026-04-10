using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using SMCV.Application.Interfaces;

namespace SMCV.Infrastructure.ExternalServices;

public class HunterService : IHunterService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    private static readonly Dictionary<string, string> _industryTagMap = new(StringComparer.OrdinalIgnoreCase)
    {
        { "tecnologia",           "5567cd4773696439b10b0000" },
        { "technology",           "5567cd4773696439b10b0000" },
        { "marketing",            "5567cd4e7369642e4ab70200" },
        { "saude",                "5567cd4773696439b10b0100" },
        { "saúde",                "5567cd4773696439b10b0100" },
        { "healthcare",           "5567cd4773696439b10b0100" },
        { "educacao",             "5567cd4773696439b10b0200" },
        { "educação",             "5567cd4773696439b10b0200" },
        { "education",            "5567cd4773696439b10b0200" },
        { "financas",             "5567cd4773696439b10b0300" },
        { "finanças",             "5567cd4773696439b10b0300" },
        { "financial services",   "5567cd4773696439b10b0300" },
        { "varejo",               "5567cd4773696439b10b0400" },
        { "retail",               "5567cd4773696439b10b0400" },
        { "logistica",            "5567cd4773696439b10b0500" },
        { "logística",            "5567cd4773696439b10b0500" },
        { "logistics",            "5567cd4773696439b10b0500" },
    };

    public HunterService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _apiKey = Environment.GetEnvironmentVariable("APOLLO_API_KEY")
            ?? throw new InvalidOperationException("Variável de ambiente 'APOLLO_API_KEY' não configurada.");
    }

    public async Task<IEnumerable<HunterContactResult>> SearchContactsAsync(
        string niche, string region, int limit = 10)
    {
        // ── Etapa 1: People Search (não consome créditos) ────────────────────
        var searchBody = new Dictionary<string, object>
        {
            ["person_locations"] = new[] { region },
            ["contact_email_status_v2"] = new[] { "verified" },
            ["per_page"] = limit,
            ["page"] = 1,
        };

        if (_industryTagMap.TryGetValue(niche, out var tagId))
        {
            searchBody["organization_industry_tag_ids"] = new[] { tagId };
        }

        var searchRequest = new HttpRequestMessage(HttpMethod.Post, "api/v1/mixed_people/api_search")
        {
            Content = JsonContent.Create(searchBody),
        };
        searchRequest.Headers.Add("x-api-key", _apiKey);

        var searchResponse = await _httpClient.SendAsync(searchRequest);

        if (!searchResponse.IsSuccessStatusCode)
        {
            var errorBody = await searchResponse.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Apollo API (search) returned {(int)searchResponse.StatusCode}: {errorBody}");
        }

        var searchJson = await searchResponse.Content.ReadFromJsonAsync<JsonElement>();
        var people = new List<(string Id, string CompanyName)>();

        if (searchJson.TryGetProperty("people", out var peopleArray))
        {
            foreach (var person in peopleArray.EnumerateArray())
            {
                var id = person.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;
                if (string.IsNullOrEmpty(id)) continue;

                var companyName = niche;
                if (person.TryGetProperty("organization", out var org) &&
                    org.TryGetProperty("name", out var orgName))
                {
                    companyName = orgName.GetString() ?? niche;
                }

                people.Add((id, companyName));
            }
        }

        // ── Etapa 2: People Enrichment (1 crédito por pessoa) ────────────────
        var results = new List<HunterContactResult>();

        foreach (var (personId, companyName) in people)
        {
            if (results.Count >= limit) break;

            var enrichBody = new
            {
                id = personId,
                reveal_personal_emails = false,
                reveal_phone_number = false,
            };

            var enrichRequest = new HttpRequestMessage(HttpMethod.Post, "api/v1/people/match")
            {
                Content = JsonContent.Create(enrichBody),
            };
            enrichRequest.Headers.Add("x-api-key", _apiKey);

            HttpResponseMessage enrichResponse;
            try
            {
                enrichResponse = await _httpClient.SendAsync(enrichRequest);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Apollo enrichment failed for person {personId}: {ex.Message}");
                continue;
            }

            if (!enrichResponse.IsSuccessStatusCode)
            {
                var errBody = await enrichResponse.Content.ReadAsStringAsync();
                Console.Error.WriteLine(
                    $"Apollo API (enrich) returned {(int)enrichResponse.StatusCode} for person {personId}: {errBody}");
                continue;
            }

            var enrichJson = await enrichResponse.Content.ReadFromJsonAsync<JsonElement>();

            string? email = null;
            if (enrichJson.TryGetProperty("person", out var enrichedPerson) &&
                enrichedPerson.TryGetProperty("email", out var emailProp))
            {
                email = emailProp.GetString();
            }

            if (!string.IsNullOrEmpty(email))
            {
                results.Add(new HunterContactResult(
                    CompanyName: companyName,
                    Email: email));
            }
        }

        return results;
    }
}
