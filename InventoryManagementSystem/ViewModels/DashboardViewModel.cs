namespace InventoryManagementSystem.ViewModels;

public class DashboardViewModel
{
    public int TotalProducts { get; set; }
    public int TotalCategories { get; set; }
    public int TotalSuppliers { get; set; }
    public int TotalStockQuantity { get; set; }
    public int LowStockCount { get; set; }

    public int TotalSalesCount { get; set; }
    public decimal TotalSalesRevenue { get; set; }

    public int TotalPurchasesCount { get; set; }
    public decimal TotalPurchasesCost { get; set; }

    public List<TopProductViewModel> MostSoldProducts { get; set; } = new();
    public List<ActivityViewModel> RecentActivities { get; set; } = new();
}

public class TopProductViewModel
{
    public string ProductName { get; set; } = string.Empty;
    public int QuantitySold { get; set; }
    public decimal Revenue { get; set; }
}

public class ActivityViewModel
{
    public DateTime Date { get; set; }

    // "Sale" or "Purchase"
    public string Type { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public string AmountDisplay { get; set; } = string.Empty;

    // "Completed" (sale) or "Received" (purchase)
    public string Status { get; set; } = string.Empty;
}
