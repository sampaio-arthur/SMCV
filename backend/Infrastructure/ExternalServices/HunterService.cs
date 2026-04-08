using System.Net.Http.Json;
using System.Text.Json;
using SMCV.Application.Interfaces;

namespace SMCV.Infrastructure.ExternalServices;

public class HunterService : IHunterService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public HunterService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _apiKey = Environment.GetEnvironmentVariable("HUNTER_API_KEY")
            ?? throw new InvalidOperationException("Variável de ambiente 'HUNTER_API_KEY' não configurada.");
    }

    public async Task<IEnumerable<HunterContactResult>> SearchContactsAsync(
        string niche, string region, int limit = 10)
    {
        var url = $"https://api.hunter.io/v2/domain-search?company={Uri.EscapeDataString(niche)}&api_key={_apiKey}&limit={limit}";

        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Hunter.io API returned {(int)response.StatusCode}: {errorBody}");
        }

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        var results = new List<HunterContactResult>();

        if (json.TryGetProperty("data", out var data) &&
            data.TryGetProperty("emails", out var emails))
        {
            var organization = data.TryGetProperty("organization", out var org) ? org.GetString() ?? niche : niche;

            foreach (var email in emails.EnumerateArray())
            {
                var emailValue = email.TryGetProperty("value", out var v) ? v.GetString() : null;
                if (string.IsNullOrEmpty(emailValue)) continue;

                results.Add(new HunterContactResult(
                    CompanyName: organization,
                    Email: emailValue));
            }
        }

        return results;
    }
}
