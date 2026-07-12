using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using TestEnhanzer.Models.External;

namespace TestEnhanzer.Services;

public record PosLoginResult(bool Success, string? Message, List<PosUserLocation> Locations);

public interface IPosApiClient
{
    Task<PosLoginResult> LoginAsync(string username, string password, CancellationToken ct = default);
}

public class PosApiClient : IPosApiClient
{
    private readonly HttpClient _http;
    private readonly ILogger<PosApiClient> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public PosApiClient(HttpClient http, ILogger<PosApiClient> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<PosLoginResult> LoginAsync(string username, string password, CancellationToken ct = default)
    {
        var request = new PosLoginRequest
        {
            ApiAction = "GetLoginData",
            DeviceId = "D001",
            SyncTime = "",
            CompanyCode = "info@enhanzer.com",
            ApiBody = new PosLoginBody
            {
                Username = "info@enhanzer.com",  //username,
                Pw = "Welcome#3" //password
            }
        };


        var json = JsonSerializer.Serialize(request, JsonOptions);
        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, (Uri?)null)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        httpRequest.Headers.ExpectContinue = false;
        httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        HttpResponseMessage httpResponse;
        try
        {
            httpResponse = await _http.SendAsync(httpRequest, HttpCompletionOption.ResponseContentRead, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reach the external POS API.");
            return new PosLoginResult(false, "Unable to reach the authentication service. Please try again later.", new());
        }

        using (httpResponse)
        {
            var raw = await httpResponse.Content.ReadAsStringAsync(ct);

            if (!httpResponse.IsSuccessStatusCode)
            {
                _logger.LogWarning("POS API returned HTTP {Status}: {Body}", (int)httpResponse.StatusCode, raw);
                return new PosLoginResult(false, "Authentication service returned an error.", new());
            }

            PosApiResponse? response;
            try
            {
                response = JsonSerializer.Deserialize<PosApiResponse>(raw, JsonOptions);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Could not parse POS API response: {Body}", raw);
                return new PosLoginResult(false, "Received an invalid response from the authentication service.", new());
            }

            if (response is null)
            {
                return new PosLoginResult(false, "Empty response from the authentication service.", new());
            }

            // The envelope carries its own status code (e.g. 401 for bad credentials).
            if (response.StatusCode is < 200 or >= 300)
            {
                var message = string.IsNullOrWhiteSpace(response.Message)
                    ? "Invalid email or password."
                    : response.Message!;
                return new PosLoginResult(false, message, new());
            }

            var locations = ExtractLocations(response.Response_Body);
            return new PosLoginResult(true, response.Message, locations);
        }
    }

    private static List<PosUserLocation> ExtractLocations(JsonElement body)
    {
        var results = new List<PosUserLocation>();

        if (body.ValueKind == JsonValueKind.String)
        {
            var inner = body.GetString();
            if (!string.IsNullOrWhiteSpace(inner))
            {
                try
                {
                    using var doc = JsonDocument.Parse(inner);
                    FindUserLocations(doc.RootElement, results);
                }
                catch (JsonException)
                {
                }
            }
            return results;
        }

        if (body.ValueKind is JsonValueKind.Object or JsonValueKind.Array)
        {
            FindUserLocations(body, results);
        }

        return results;
    }

    private static void FindUserLocations(JsonElement element, List<PosUserLocation> results)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var property in element.EnumerateObject())
                {
                    if (property.NameEquals("User_Locations") && property.Value.ValueKind == JsonValueKind.Array)
                    {
                        MapLocationArray(property.Value, results);
                    }
                    else
                    {
                        FindUserLocations(property.Value, results);
                    }
                }
                break;

            case JsonValueKind.Array:
                foreach (var item in element.EnumerateArray())
                {
                    FindUserLocations(item, results);
                }
                break;
        }
    }

    private static void MapLocationArray(JsonElement array, List<PosUserLocation> results)
    {
        foreach (var item in array.EnumerateArray())
        {
            if (item.ValueKind != JsonValueKind.Object)
            {
                continue;
            }

            var location = new PosUserLocation
            {
                LocationCode = GetStringProperty(item, "Location_Code"),
                LocationName = GetStringProperty(item, "Location_Name")
            };

            if (!string.IsNullOrWhiteSpace(location.LocationCode) || !string.IsNullOrWhiteSpace(location.LocationName))
            {
                results.Add(location);
            }
        }
    }

    private static string GetStringProperty(JsonElement obj, string name)
    {
        foreach (var property in obj.EnumerateObject())
        {
            if (property.NameEquals(name))
            {
                return property.Value.ValueKind switch
                {
                    JsonValueKind.String => property.Value.GetString() ?? string.Empty,
                    JsonValueKind.Number => property.Value.ToString(),
                    _ => string.Empty
                };
            }
        }
        return string.Empty;
    }
}
