namespace InventoryManagementSystem.Models;

public class Purchase
{
    public int PurchaseId { get; set; }

    public int SupplierId { get; set; }

    public DateTime PurchaseDate { get; set; } = DateTime.Now;

    public decimal TotalAmount { get; set; }

    public Supplier Supplier { get; set; } = null!;
    public ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();
}