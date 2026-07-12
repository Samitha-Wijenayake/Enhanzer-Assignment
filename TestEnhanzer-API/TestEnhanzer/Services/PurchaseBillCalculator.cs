using TestEnhanzer.Models.Dtos;

namespace TestEnhanzer.Services;

public interface IPurchaseBillCalculator
{
    PurchaseBillLineResultDto CalculateLine(PurchaseBillLineDto line);
    PurchaseBillSummaryDto Summarize(IEnumerable<PurchaseBillLineDto> lines);
}

public class PurchaseBillCalculator : IPurchaseBillCalculator
{
    public PurchaseBillLineResultDto CalculateLine(PurchaseBillLineDto line)
    {
        var grossCost = line.StandardCost * line.Quantity;
        var discountFactor = 1 - (line.DiscountPercentage / 100m);
        var totalCost = Math.Round(grossCost * discountFactor, 2, MidpointRounding.AwayFromZero);
        var totalSelling = Math.Round(line.StandardPrice * line.Quantity, 2, MidpointRounding.AwayFromZero);

        return new PurchaseBillLineResultDto
        {
            Item = line.Item,
            Batch = line.Batch,
            StandardCost = line.StandardCost,
            StandardPrice = line.StandardPrice,
            Quantity = line.Quantity,
            DiscountPercentage = line.DiscountPercentage,
            TotalCost = totalCost,
            TotalSelling = totalSelling
        };
    }

    public PurchaseBillSummaryDto Summarize(IEnumerable<PurchaseBillLineDto> lines)
    {
        var calculated = lines.Select(CalculateLine).ToList();

        return new PurchaseBillSummaryDto
        {
            TotalItems = calculated.Count,
            TotalQuantity = calculated.Sum(l => l.Quantity),
            TotalCost = calculated.Sum(l => l.TotalCost),
            TotalSelling = calculated.Sum(l => l.TotalSelling)
        };
    }
}
