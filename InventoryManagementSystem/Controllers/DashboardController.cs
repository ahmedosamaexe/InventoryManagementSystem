using InventoryManagementSystem.ViewModels;

namespace InventoryManagementSystem.Controllers;

public class DashboardController : Controller
{
    private readonly AppDbContext _context;

    public DashboardController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(DateTime? from, DateTime? to)
    {
        var toDate = (to ?? DateTime.Today).Date;
        var fromDate = (from ?? toDate.AddDays(-6)).Date;
        if (fromDate > toDate)
            (fromDate, toDate) = (toDate, fromDate);

        var rangeEnd = toDate.AddDays(1);

        var vm = new DashboardViewModel
        {
            From = fromDate,
            To = toDate,

            SalesTotal = await _context.Sales
                .Where(s => s.SaleDate >= fromDate && s.SaleDate < rangeEnd)
                .SumAsync(s => (decimal?)s.TotalAmount) ?? 0,
            PurchasesTotal = await _context.Purchases
                .Where(p => p.PurchaseDate >= fromDate && p.PurchaseDate < rangeEnd)
                .SumAsync(p => (decimal?)p.TotalAmount) ?? 0,
            InvoiceCount = await _context.Sales
                .CountAsync(s => s.SaleDate >= fromDate && s.SaleDate < rangeEnd),

            TotalProducts = await _context.Products.CountAsync(),
            TotalCategories = await _context.Categories.CountAsync(),
            TotalSuppliers = await _context.Suppliers.CountAsync(),
            TotalStockQuantity = await _context.Products.SumAsync(p => (int?)p.StockQuantity) ?? 0,
            LowStockCount = await _context.Products
                .CountAsync(p => p.StockQuantity <= p.LowStockThreshold)
        };

        vm.Profit = vm.SalesTotal - vm.PurchasesTotal;

        // Daily series for the charts (zero-filled days included).
        var salesByDay = await _context.Sales
            .AsNoTracking()
            .Where(s => s.SaleDate >= fromDate && s.SaleDate < rangeEnd)
            .GroupBy(s => s.SaleDate.Date)
            .Select(g => new { Day = g.Key, Total = g.Sum(s => s.TotalAmount) })
            .ToListAsync();

        var purchasesByDay = await _context.Purchases
            .AsNoTracking()
            .Where(p => p.PurchaseDate >= fromDate && p.PurchaseDate < rangeEnd)
            .GroupBy(p => p.PurchaseDate.Date)
            .Select(g => new { Day = g.Key, Total = g.Sum(p => p.TotalAmount) })
            .ToListAsync();

        for (var day = fromDate; day <= toDate; day = day.AddDays(1))
        {
            vm.DailySeries.Add(new DailyPointViewModel
            {
                Label = day.ToString("yyyy-MM-dd"),
                Sales = salesByDay.FirstOrDefault(x => x.Day == day)?.Total ?? 0,
                Purchases = purchasesByDay.FirstOrDefault(x => x.Day == day)?.Total ?? 0
            });
        }

        // Top selling products: year of `to` and month of `to`.
        vm.TopSellingYear = await _context.SaleItems
            .AsNoTracking()
            .Where(si => si.Sale.SaleDate.Year == toDate.Year)
            .GroupBy(si => new { si.ProductId, si.Product.ProductName })
            .Select(g => new TopProductViewModel
            {
                ProductName = g.Key.ProductName,
                QuantitySold = g.Sum(x => x.Quantity),
                Revenue = g.Sum(x => x.Quantity * x.UnitPrice)
            })
            .OrderByDescending(x => x.QuantitySold)
            .Take(5)
            .ToListAsync();

        vm.TopSellingMonth = await _context.SaleItems
            .AsNoTracking()
            .Where(si => si.Sale.SaleDate.Year == toDate.Year && si.Sale.SaleDate.Month == toDate.Month)
            .GroupBy(si => new { si.ProductId, si.Product.ProductName })
            .Select(g => new TopProductViewModel
            {
                ProductName = g.Key.ProductName,
                QuantitySold = g.Sum(x => x.Quantity),
                Revenue = g.Sum(x => x.Quantity * x.UnitPrice)
            })
            .OrderByDescending(x => x.QuantitySold)
            .Take(5)
            .ToListAsync();

        // Top customers of the selected month.
        vm.TopCustomers = await _context.Sales
            .AsNoTracking()
            .Where(s => s.SaleDate.Year == toDate.Year && s.SaleDate.Month == toDate.Month)
            .GroupBy(s => s.CustomerInfo ?? "walk-in-customer")
            .Select(g => new TopCustomerViewModel
            {
                Customer = g.Key,
                Orders = g.Count(),
                Total = g.Sum(s => s.TotalAmount)
            })
            .OrderByDescending(x => x.Total)
            .Take(5)
            .ToListAsync();

        // Stock alerts.
        vm.StockAlerts = await _context.Products
            .AsNoTracking()
            .Where(p => p.StockQuantity <= p.LowStockThreshold)
            .OrderBy(p => p.StockQuantity)
            .Take(10)
            .Select(p => new StockAlertViewModel
            {
                SKU = p.SKU,
                ProductName = p.ProductName,
                StockQuantity = p.StockQuantity,
                AlertQuantity = p.LowStockThreshold
            })
            .ToListAsync();

        // Recent Activity Log: latest sales + incoming purchases merged by date.
        var activitySales = await _context.Sales
            .AsNoTracking()
            .Include(s => s.SaleItems)
                .ThenInclude(si => si.Product)
            .OrderByDescending(s => s.SaleDate)
            .Take(5)
            .ToListAsync();

        var activityPurchases = await _context.Purchases
            .AsNoTracking()
            .Include(p => p.Supplier)
            .Include(p => p.PurchaseItems)
            .OrderByDescending(p => p.PurchaseDate)
            .Take(5)
            .ToListAsync();

        vm.RecentActivities = activitySales
            .Select(s => new ActivityViewModel
            {
                Date = s.SaleDate,
                Type = "Sale",
                Details = $"Order #SL-{s.SaleId}" +
                    (s.SaleItems.FirstOrDefault()?.Product is { } product
                        ? $" ({product.ProductName}{(s.SaleItems.Count > 1 ? $" +{s.SaleItems.Count - 1} more" : "")})"
                        : ""),
                AmountDisplay = $"-${s.TotalAmount:0.##} (Qty: {s.SaleItems.Sum(i => i.Quantity)})",
                Status = "Completed"
            })
            .Concat(activityPurchases.Select(p => new ActivityViewModel
            {
                Date = p.PurchaseDate,
                Type = "Purchase",
                Details = $"PO #PR-{p.PurchaseId} ({p.Supplier.SupplierName})",
                AmountDisplay = $"+${p.TotalAmount:0.##} (Qty: {p.PurchaseItems.Sum(i => i.Quantity)})",
                Status = "Received"
            }))
            .OrderByDescending(a => a.Date)
            .Take(10)
            .ToList();

        return View(vm);
    }
}
