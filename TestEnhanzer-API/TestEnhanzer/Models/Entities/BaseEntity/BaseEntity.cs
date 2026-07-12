using System.ComponentModel.DataAnnotations;

namespace TestEnhanzer.Models.Entities;

public class BaseEntity
{
    public int Id { get; set; }

    public DateTime CreatedAtUTC { get; set; } = DateTime.UtcNow;
    public DateTime UpdateAtUTC { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
}
