namespace InventoryManagementSystem.Models;

public class SaleItem
{
    public int SaleItemId { get; set; }

    public int SaleId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    // Navigation
    [ValidateNever]
    public Sale Sale { get; set; } = null!;
    [ValidateNever]
    public Product Product { get; set; } = null!;
}