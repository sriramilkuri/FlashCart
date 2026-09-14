namespace FlashCart.Domain.Services;

public class CartPricingService
{
    public const decimal TaxRate = 0.18m;

    public decimal CalculateSubtotal(
        decimal price,
        int quantity)
    {
        return decimal.Round(
            price * quantity,
            2,
            MidpointRounding.AwayFromZero);
    }

    public decimal CalculateTax(
        decimal subtotal)
    {
        return decimal.Round(
            subtotal * TaxRate,
            2,
            MidpointRounding.AwayFromZero);
    }

    public decimal CalculateTotal(
        decimal subtotal,
        decimal tax)
    {
        return decimal.Round(
            subtotal + tax,
            2,
            MidpointRounding.AwayFromZero);
    }
}