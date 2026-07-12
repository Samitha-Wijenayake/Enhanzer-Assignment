using System.Text.Json;
using System.Text.Json.Serialization;

namespace TestEnhanzer.Models.External;

/// <summary>
/// Request body sent to the external Enhanzer POS login endpoint.
/// Per the assignment: Company_Code and the nested API_Body.Username are both
/// the email field, and API_Body.Pw is the password field.
/// </summary>
public class PosLoginRequest
{
    [JsonPropertyName("API_Action")]
    public string ApiAction { get; set; } = "GetLoginData";

    [JsonPropertyName("Device_Id")]
    public string DeviceId { get; set; } = "D001";

    [JsonPropertyName("Sync_Time")]
    public string SyncTime { get; set; } = string.Empty;

    [JsonPropertyName("Company_Code")]
    public string CompanyCode { get; set; } = string.Empty;

    [JsonPropertyName("API_Body")]
    public PosLoginBody ApiBody { get; set; } = new();
}

/// <summary>Nested credentials block of the POS login request.</summary>
public class PosLoginBody
{
    [JsonPropertyName("Username")]
    public string Username { get; set; } = string.Empty;

    [JsonPropertyName("Pw")]
    public string Pw { get; set; } = string.Empty;
}

/// <summary>Envelope returned by the external POS API.</summary>
public class PosApiResponse
{
    [JsonPropertyName("Status_Code")]
    public int StatusCode { get; set; }

    [JsonPropertyName("Sync_Time")]
    public string? SyncTime { get; set; }

    [JsonPropertyName("Message")]
    public string? Message { get; set; }

    // Response_Body shape is not strongly documented, so keep it flexible.
    [JsonPropertyName("Response_Body")]
    public JsonElement Response_Body { get; set; }
}

/// <summary>A user location as returned inside the POS API response body.</summary>
public class PosUserLocation
{
    [JsonPropertyName("Location_Code")]
    public string LocationCode { get; set; } = string.Empty;

    [JsonPropertyName("Location_Name")]
    public string LocationName { get; set; } = string.Empty;
}
