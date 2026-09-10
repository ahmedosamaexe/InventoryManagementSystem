namespace InventoryManagementSystem.Models;

public class Category
{
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Category name is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Category name must be between 2 and 100 characters.")]
    [Display(Name = "Category Name")]
    public string CategoryName { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    [Display(Name = "Description")]
    public string? Description { get; set; }

    // Navigation
    public ICollection<Product> Products { get; set; } = new List<Product>();
}