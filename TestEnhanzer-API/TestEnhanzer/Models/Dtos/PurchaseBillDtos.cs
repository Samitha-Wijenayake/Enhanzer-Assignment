using System.ComponentModel.DataAnnotations;

namespace TestEnhanzer.Models.Dtos;

/// <summary>A single purchase bill line submitted from the Angular form.</summary>
public class PurchaseBillLineDto
{
    [Required(ErrorMessage = "Item is required.")]
    public string Item { get; set; } = string.Empty;

    [Required(ErrorMessage = "Batch (location) is required.")]
    public string Batch { get; set; } = string.Empty;

    [Range(0, double.MaxValue, ErrorMessage = "Standard cost must be zero or greater.")]
    public decimal StandardCost { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Standard price must be zero or greater.")]
    public decimal StandardPrice { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
    public int Quantity { get; set; }

    [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100.")]
    public decimal DiscountPercentage { get; set; }
}

/// <summary>A calculated purchase bill line returned to the client.</summary>
public class PurchaseBillLineResultDto : PurchaseBillLineDto
{
    public decimal TotalCost { get; set; }
    public decimal TotalSelling { get; set; }
}

/// <summary>Aggregated summary of all lines on the bill.</summary>
public class PurchaseBillSummaryDto
{
    public int TotalItems { get; set; }
    public int TotalQuantity { get; set; }
    public decimal TotalCost { get; set; }
    public decimal TotalSelling { get; set; }
}
