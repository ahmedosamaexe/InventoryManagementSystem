using System.Net.ServerSentEvents;

namespace InventoryManagementSystem.Models;

public class Product
{
    public int ProductId { get; set; }

    [Required]
    [MaxLength(50)]
    public string SKU { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string ProductName { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public decimal UnitPrice { get; set; }

    public int StockQuantity { get; set; } = 0;

    public int LowStockThreshold { get; set; } = 0;

    // Navigation
    public Category Category { get; set; } = null!;

    public ICollection<SupplierProduct> SupplierProducts { get; set; }
    = new List<SupplierProduct>();

    public ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();
    public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
}