using InventoryManagementSystem.ViewModels;

namespace InventoryManagementSystem.Controllers;

public class DashboardController : Controller
{
    private readonly AppDbContext _context;

    public DashboardController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var vm = new DashboardViewModel
        {
            TotalProducts = await _context.Products.CountAsync(),
            TotalCategories = await _context.Categories.CountAsync(),
            TotalSuppliers = await _context.Suppliers.CountAsync(),
            TotalStockQuantity = await _context.Products.SumAsync(p => (int?)p.StockQuantity) ?? 0,
            LowStockCount = await _context.Products
                .CountAsync(p => p.StockQuantity <= p.LowStockThreshold),

            TotalSalesCount = await _context.Sales.CountAsync(),
            TotalSalesRevenue = await _context.Sales.SumAsync(s => (decimal?)s.TotalAmount) ?? 0,

            TotalPurchasesCount = await _context.Purchases.CountAsync(),
            TotalPurchasesCost = await _context.Purchases.SumAsync(p => (decimal?)p.TotalAmount) ?? 0,

            MostSoldProducts = await _context.SaleItems
                .GroupBy(si => new { si.ProductId, si.Product.ProductName })
                .Select(g => new TopProductViewModel
                {
                    ProductName = g.Key.ProductName,
                    QuantitySold = g.Sum(x => x.Quantity),
                    Revenue = g.Sum(x => x.Quantity * x.UnitPrice)
                })
                .OrderByDescending(x => x.QuantitySold)
                .Take(5)
                .ToListAsync()
        };

        var recentSales = await _context.Sales
            .AsNoTracking()
            .Include(s => s.SaleItems)
                .ThenInclude(si => si.Product)
            .OrderByDescending(s => s.SaleDate)
            .Take(5)
            .ToListAsync();

        var recentPurchases = await _context.Purchases
            .AsNoTracking()
            .Include(p => p.Supplier)
            .Include(p => p.PurchaseItems)
            .OrderByDescending(p => p.PurchaseDate)
            .Take(5)
            .ToListAsync();

        var activities = recentSales
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
            .Concat(recentPurchases.Select(p => new ActivityViewModel
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

        vm.RecentActivities = activities;

        return View(vm);
    }
}
