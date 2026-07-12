using System.ComponentModel.DataAnnotations;

namespace TestEnhanzer.Models.Dtos;

public class LoginRequestDto
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "A valid email address is required.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(1, ErrorMessage = "Password cannot be empty.")]
    public string Password { get; set; } = string.Empty;
}

public class LocationDto
{
    public string LocationCode { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
}

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
    public string Username { get; set; } = string.Empty;
    public List<LocationDto> Locations { get; set; } = new();
}
