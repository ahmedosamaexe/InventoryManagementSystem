namespace InventoryManagementSystem.Models;

public class Product
{
    public int ProductId { get; set; }

    [Required(ErrorMessage = "SKU is required.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "SKU must be between 3 and 50 characters.")]
    [RegularExpression(@"^[A-Za-z0-9\-]+$", ErrorMessage = "SKU can only contain letters, numbers, and hyphens.")]
    [Display(Name = "SKU")]
    public string SKU { get; set; } = string.Empty;

    [Required(ErrorMessage = "Product name is required.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Product name must be between 2 and 150 characters.")]
    [Display(Name = "Product Name")]
    public string ProductName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please select a category.")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a valid category.")]
    [Display(Name = "Category")]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Unit price is required.")]
    [Range(0.01, 9999999.99, ErrorMessage = "Unit price must be between 0.01 and 9,999,999.99.")]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Unit Price")]
    public decimal UnitPrice { get; set; }

    [Required(ErrorMessage = "Stock quantity is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative.")]
    [Display(Name = "Stock Quantity")]
    public int StockQuantity { get; set; } = 0;

    [Required(ErrorMessage = "Low stock threshold is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "Low stock threshold cannot be negative.")]
    [Display(Name = "Low Stock Threshold")]
    public int LowStockThreshold { get; set; } = 0;

    // Navigation
    [ValidateNever]
    [BindNever]
    public Category Category { get; set; } = null!;

    [ValidateNever]
    [BindNever]
    public ICollection<SupplierProduct> SupplierProducts { get; set; }
    = new List<SupplierProduct>();

    [ValidateNever]
    [BindNever]
    public ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();
    [ValidateNever]
    [BindNever]
    public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
}