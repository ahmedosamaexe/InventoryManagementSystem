namespace InventoryManagementSystem.Services;

public class StockService
{
    private readonly AppDbContext _context;

    public StockService(AppDbContext context)
    {
        _context = context;
    }

    // Called when a new Purchase is added
    public void ProcessPurchase(Purchase purchase)
    {
        decimal total = 0;

        foreach (var item in purchase.PurchaseItems)
        {
            var product = _context.Products.Find(item.ProductId)
                ?? throw new InvalidOperationException($"Product not found (ID: {item.ProductId})");

            product.StockQuantity += item.Quantity;
            total += item.Quantity * item.UnitCost;
        }

        purchase.TotalAmount = total;
    }

    // Called when a new Sale is added
    public void ProcessSale(Sale sale)
    {
        decimal total = 0;

        foreach (var item in sale.SaleItems)
        {
            var product = _context.Products.Find(item.ProductId)
                ?? throw new InvalidOperationException($"Product not found (ID: {item.ProductId})");

            if (product.StockQuantity < item.Quantity)
                throw new InvalidOperationException(
                    $"Insufficient stock for \"{product.ProductName}\" (available: {product.StockQuantity})");

            product.StockQuantity -= item.Quantity;
            total += item.Quantity * item.UnitPrice;
        }

        sale.TotalAmount = total;
    }
}