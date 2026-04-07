using System.Net.Http.Json;
using System.Text.Json;
using SMCV.Application.Interfaces;

namespace SMCV.Infrastructure.ExternalServices;

public class HunterService : IHunterService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public HunterService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["HunterApi:ApiKey"]
            ?? throw new InvalidOperationException("HunterApi:ApiKey not configured in appsettings.json");
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
            var domain = data.TryGetProperty("domain", out var d) ? d.GetString() ?? "" : "";
            var organization = data.TryGetProperty("organization", out var org) ? org.GetString() ?? niche : niche;

            foreach (var email in emails.EnumerateArray())
            {
                var emailValue = email.TryGetProperty("value", out var v) ? v.GetString() : null;
                if (string.IsNullOrEmpty(emailValue)) continue;

                var firstName = email.TryGetProperty("first_name", out var fn) ? fn.GetString() : null;
                var lastName = email.TryGetProperty("last_name", out var ln) ? ln.GetString() : null;
                var contactName = (firstName, lastName) switch
                {
                    (not null, not null) => $"{firstName} {lastName}",
                    (not null, null) => firstName,
                    (null, not null) => lastName,
                    _ => null
                };
                var position = email.TryGetProperty("position", out var pos) ? pos.GetString() : null;

                results.Add(new HunterContactResult(
                    CompanyName: organization,
                    Domain: domain,
                    Email: emailValue,
                    ContactName: contactName,
                    Position: position));
            }
        }

        return results;
    }
}
