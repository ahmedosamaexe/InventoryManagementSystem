namespace InventoryManagementSystem.ViewModels;

public class ProductIndexViewModel
{
    // Data
    public List<Product> Products { get; set; } = new();
    public List<Category> Categories { get; set; } = new();

    // Search & Filter
    public string? SearchTerm { get; set; }
    public int? CategoryId { get; set; }
    public string? StockStatus { get; set; }

    // Pagination
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; }
    public int PageSize { get; set; } = 10;
    public int TotalCount { get; set; }

    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
}
