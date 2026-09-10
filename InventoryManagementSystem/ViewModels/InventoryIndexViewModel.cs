namespace InventoryManagementSystem.ViewModels;

public class InventoryIndexViewModel
{
    public List<Product> Products { get; set; } = new();

    public int TotalProducts { get; set; }

    public int TotalUnits { get; set; }

    public int LowStockCount { get; set; }

    public int OutOfStockCount { get; set; }

    public string? SearchTerm { get; set; }

    public string? StockStatus { get; set; }
}