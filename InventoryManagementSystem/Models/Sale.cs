namespace InventoryManagementSystem.Models;

public class Sale
{
    public int SaleId { get; set; }

    [Required(ErrorMessage = "Sale date is required.")]
    [DataType(DataType.Date)]
    [Display(Name = "Sale Date")]
    public DateTime SaleDate { get; set; } = DateTime.Now;

    [Range(0, 9999999999.99, ErrorMessage = "Total amount must be a valid positive value.")]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Total Amount")]
    public decimal TotalAmount { get; set; }

    [StringLength(150, ErrorMessage = "Customer info cannot exceed 150 characters.")]
    [Display(Name = "Customer Info")]
    public string? CustomerInfo { get; set; }

    // Navigation
    [ValidateNever]
    public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
}