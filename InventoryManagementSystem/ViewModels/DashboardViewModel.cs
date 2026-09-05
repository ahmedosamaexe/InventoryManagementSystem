namespace InventoryManagementSystem.ViewModels;

public class DashboardViewModel
{
    public DateTime From { get; set; }
    public DateTime To { get; set; }

    // KPI cards (range-based)
    public decimal SalesTotal { get; set; }
    public decimal PurchasesTotal { get; set; }
    public int InvoiceCount { get; set; }
    public decimal Profit { get; set; }

    // Catalog summary (all-time)
    public int TotalProducts { get; set; }
    public int TotalCategories { get; set; }
    public int TotalSuppliers { get; set; }
    public int TotalStockQuantity { get; set; }
    public int LowStockCount { get; set; }

    // Charts
    public List<DailyPointViewModel> DailySeries { get; set; } = new();
    public List<TopProductViewModel> TopSellingYear { get; set; } = new();
    public List<TopProductViewModel> TopSellingMonth { get; set; } = new();
    public List<TopCustomerViewModel> TopCustomers { get; set; } = new();

    // Tables
    public List<StockAlertViewModel> StockAlerts { get; set; } = new();
    public List<ActivityViewModel> RecentActivities { get; set; } = new();
}

public class DailyPointViewModel
{
    public string Label { get; set; } = string.Empty;
    public decimal Sales { get; set; }
    public decimal Purchases { get; set; }
}

public class TopProductViewModel
{
    public string ProductName { get; set; } = string.Empty;
    public int QuantitySold { get; set; }
    public decimal Revenue { get; set; }
}

public class TopCustomerViewModel
{
    public string Customer { get; set; } = string.Empty;
    public int Orders { get; set; }
    public decimal Total { get; set; }
}

public class StockAlertViewModel
{
    public string SKU { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
    public int AlertQuantity { get; set; }
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
