using Microsoft.EntityFrameworkCore;
using TestEnhanzer.Data;
using TestEnhanzer.Models.Dtos;

namespace TestEnhanzer.Services;

public interface ILocationService
{
    Task<List<LocationDto>> GetLocationsAsync(string username, CancellationToken ct = default);
}

public class LocationService : ILocationService
{
    private readonly AppDbContext _db;

    public LocationService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<LocationDto>> GetLocationsAsync(string username, CancellationToken ct = default)
    {
        return await _db.UserLocation
            .Where(l => l.UserDetails.Username == username)
            .OrderBy(l => l.LocationName)
            .Select(l => new LocationDto
            {
                LocationCode = l.LocationCode,
                LocationName = l.LocationName
            })
            .ToListAsync(ct);
    }
}
