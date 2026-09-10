namespace InventoryManagementSystem.Controllers;

public class InventoryController : Controller
{
    private readonly AppDbContext _context;

    public InventoryController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Inventory
    public async Task<IActionResult> Index(
        string? search,
        string? stockStatus)
    {
        IQueryable<Product> query = _context.Products
            .Include(p => p.Category);

        // Search by product name or SKU
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();

            query = query.Where(p =>
                p.ProductName.ToLower().Contains(term) ||
                p.SKU.ToLower().Contains(term));
        }

        // Filter by stock status
        if (!string.IsNullOrWhiteSpace(stockStatus))
        {
            query = stockStatus switch
            {
                "InStock" =>
                    query.Where(p =>
                        p.StockQuantity > p.LowStockThreshold),

                "LowStock" =>
                    query.Where(p =>
                        p.StockQuantity > 0 &&
                        p.StockQuantity <= p.LowStockThreshold),

                "OutOfStock" =>
                    query.Where(p =>
                        p.StockQuantity == 0),

                _ => query
            };
        }

        var products = await query
            .OrderBy(p => p.ProductName)
            .ToListAsync();

        var totalProducts = await _context.Products.CountAsync();

        var totalUnits = await _context.Products
            .SumAsync(p => p.StockQuantity);

        var lowStockCount = await _context.Products
            .CountAsync(p =>
                p.StockQuantity > 0 &&
                p.StockQuantity <= p.LowStockThreshold);

        var outOfStockCount = await _context.Products
            .CountAsync(p =>
                p.StockQuantity == 0);

        var viewModel = new ViewModels.InventoryIndexViewModel
        {
            Products = products,

            TotalProducts = totalProducts,

            TotalUnits = totalUnits,

            LowStockCount = lowStockCount,

            OutOfStockCount = outOfStockCount,

            SearchTerm = search,

            StockStatus = stockStatus
        };

        return View(viewModel);
    }
}