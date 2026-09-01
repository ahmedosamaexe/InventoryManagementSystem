namespace InventoryManagementSystem.Models;

public class Supplier
{
    public int SupplierId { get; set; }

    [Required]
    [MaxLength(100)]
    public string SupplierName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? ContactName { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(100)]
    public string? Email { get; set; }

    [MaxLength(250)]
    public string? Address { get; set; }

    // Navigation
    public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
}