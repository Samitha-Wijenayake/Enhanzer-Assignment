using System.ComponentModel.DataAnnotations;

namespace TestEnhanzer.Models.Entities;
 
public class UserDetails: BaseEntity
{

    [MaxLength(150)]
    public string Username { get; set; } = string.Empty;
    public List<UserLocation> UserLocations { get; set; } = new List<UserLocation>();

}
