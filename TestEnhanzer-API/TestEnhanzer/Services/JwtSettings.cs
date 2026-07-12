namespace TestEnhanzer.Services;

public class JwtSettings
{
    public string Issuer { get; set; } = "TestEnhanzer";
    public string Audience { get; set; } = "TestEnhanzerClient";
    public string Key { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; } = 120;
}
