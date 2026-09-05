namespace InventoryManagementSystem.Models;

public class PurchaseItem
{
    public int PurchaseItemId { get; set; }

    public int PurchaseId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitCost { get; set; }

    [ValidateNever]
    public Purchase Purchase { get; set; } = null!;
    [ValidateNever]
    public Product Product { get; set; } = null!;
}