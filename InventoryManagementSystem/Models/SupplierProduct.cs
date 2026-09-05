namespace InventoryManagementSystem.Models;

public class SupplierProduct
{
    public int SupplierId { get; set; }

    public int ProductId { get; set; }


    [ValidateNever]
    public Supplier Supplier { get; set; } = null!;

    [ValidateNever]
    public Product Product { get; set; } = null!;
}