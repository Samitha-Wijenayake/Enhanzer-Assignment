using System.ComponentModel.DataAnnotations;

namespace TestEnhanzer.Models.Entities;

public class UserLocation : BaseEntity
{
    public int UserDetailsId { get; set; }
    public UserDetails UserDetails { get; set; }

    public string LocationCode { get; set; }

    [MaxLength(200)]
    public string LocationName { get; set; }

}
