namespace InventoryManagementSystem.Models;

public class PurchaseItem
{
    public int PurchaseItemId { get; set; }

    [Required]
    public int PurchaseId { get; set; }

    [Required(ErrorMessage = "Please select a product.")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a valid product.")]
    [Display(Name = "Product")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "Quantity is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
    [Display(Name = "Quantity")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "Unit cost is required.")]
    [Range(0.01, 9999999.99, ErrorMessage = "Unit cost must be between 0.01 and 9,999,999.99.")]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Unit Cost")]
    public decimal UnitCost { get; set; }

    [ValidateNever]
    public Purchase Purchase { get; set; } = null!;
    [ValidateNever]
    public Product Product { get; set; } = null!;
}