using Microsoft.EntityFrameworkCore;
using TestEnhanzer.Data;
using TestEnhanzer.Models.Dtos;
using TestEnhanzer.Models.Entities;
using TestEnhanzer.Models.External;

namespace TestEnhanzer.Services;

public record AuthOutcome(bool Success, string? ErrorMessage, LoginResponseDto? Data);

public interface IAuthService
{
    Task<AuthOutcome> LoginAsync(LoginRequestDto request, CancellationToken ct = default);
}

public class AuthService : IAuthService
{
    private readonly IPosApiClient _posApi;
    private readonly ITokenService _tokenService;
    private readonly AppDbContext _db;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IPosApiClient posApi,
        ITokenService tokenService,
        AppDbContext db,
        ILogger<AuthService> logger)
    {
        _posApi = posApi;
        _tokenService = tokenService;
        _db = db;
        _logger = logger;
    }

    public async Task<AuthOutcome> LoginAsync(LoginRequestDto request, CancellationToken ct = default)
    {
        var email = request.Email.Trim();

        // ✅ Call external POS API
        var result = await _posApi.LoginAsync(email, request.Password, ct);

        if (!result.Success)
        {
            return new AuthOutcome(false, result.Message ?? "Invalid email or password.", null);
        }

        // ✅ Persist user + locations in SQL Server
        await PersistLocationsAsync(email, result.Locations, ct);

        // ✅ Generate JWT
        var (token, expiresAt) = _tokenService.CreateToken(email);

        var data = new LoginResponseDto
        {
            Token = token,
            ExpiresAtUtc = expiresAt,
            Username = email,
            Locations = result.Locations
                .Select(l => new LocationDto
                {
                    LocationCode = l.LocationCode,
                    LocationName = l.LocationName
                })
                .ToList()
        };

        return new AuthOutcome(true, null, data);
    }

    /// <summary>
    /// Creates or updates the user and synchronizes their locations.
    /// </summary>
    private async Task PersistLocationsAsync(
        string username,
        List<PosUserLocation> locations,
        CancellationToken ct)
    {
        if (locations == null || locations.Count == 0)
            return;

        try
        {
            var user = await _db.UserDetails
                .Include(u => u.UserLocations)
                .FirstOrDefaultAsync(u => u.Username == username, ct);

            if (user == null)
            {
                // ✅ Create new user
                user = new UserDetails
                {
                    Username = username,
                    CreatedAtUTC = DateTime.UtcNow,
                    UpdateAtUTC = DateTime.UtcNow,
                    IsDeleted = false,
                    UserLocations = new List<UserLocation>()
                };

                _db.UserDetails.Add(user);
            }
            else
            {
                // ✅ Remove old locations (clean re-sync)
                user.UserLocations.Clear();

                user.UpdateAtUTC = DateTime.UtcNow;
            }

            // ✅ Insert fresh locations
            foreach (var loc in locations)
            {
                user.UserLocations.Add(new UserLocation
                {
                    LocationCode = loc.LocationCode,
                    LocationName = loc.LocationName,
                    CreatedAtUTC = DateTime.UtcNow,
                    UpdateAtUTC = DateTime.UtcNow,
                    IsDeleted = false
                });
            }

            await _db.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to persist locations for {Username}.", username);
        }
    }
}