namespace InventoryManagementSystem.Models;

public class Purchase
{
    public int PurchaseId { get; set; }

    [Required(ErrorMessage = "Please select a supplier.")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a valid supplier.")]
    [Display(Name = "Supplier")]
    public int SupplierId { get; set; }

    [Required(ErrorMessage = "Purchase date is required.")]
    [DataType(DataType.Date)]
    [Display(Name = "Purchase Date")]
    public DateTime PurchaseDate { get; set; } = DateTime.Now;

    [Range(0, 9999999999.99, ErrorMessage = "Total amount must be a valid positive value.")]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Total Amount")]
    public decimal TotalAmount { get; set; }

    [ValidateNever]
    public Supplier Supplier { get; set; } = null!;
    [ValidateNever]
    public ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();
}