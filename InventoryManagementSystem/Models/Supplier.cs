namespace InventoryManagementSystem.Models;

public class Supplier
{
    public int SupplierId { get; set; }


    [Required(ErrorMessage = "Supplier name is required.")]
    [StringLength(100, ErrorMessage = "Supplier name cannot exceed 100 characters.")]
    public string SupplierName { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "Contact name cannot exceed 100 characters.")]
    public string? ContactName { get; set; }


    [Display(Name = "Phone")]
    [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters.")]
    [RegularExpression(@"^[0-9+\-\s()]{7,20}$",
        ErrorMessage = "Please enter a valid phone number.")]
    public string? Phone { get; set; }

    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
    public string? Email { get; set; }

    [StringLength(250, ErrorMessage = "Address cannot exceed 250 characters.")]
    public string? Address { get; set; }

    public ICollection<SupplierProduct> SupplierProducts { get; set; }
    = new List<SupplierProduct>();

    public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
}