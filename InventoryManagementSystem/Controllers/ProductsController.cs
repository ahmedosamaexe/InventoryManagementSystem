namespace InventoryManagementSystem.Controllers;

public class ProductsController : Controller
{
    private readonly AppDbContext _context;

    public ProductsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Products
    public async Task<IActionResult> Index(string? search, int? categoryId, string? stockStatus, int page = 1)
    {
        const int pageSize = 10;

        IQueryable<Product> query = _context.Products.Include(p => p.Category);

        // Search by ProductName or SKU
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(p =>
                p.ProductName.ToLower().Contains(term) ||
                p.SKU.ToLower().Contains(term));
        }

        // Filter by Category
        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        // Filter by Stock Status
        if (!string.IsNullOrWhiteSpace(stockStatus))
        {
            query = stockStatus switch
            {
                "InStock" => query.Where(p => p.StockQuantity > p.LowStockThreshold),
                "LowStock" => query.Where(p => p.StockQuantity > 0 && p.StockQuantity <= p.LowStockThreshold),
                "OutOfStock" => query.Where(p => p.StockQuantity == 0),
                _ => query
            };
        }

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        // Clamp page
        if (page < 1) page = 1;
        if (page > totalPages && totalPages > 0) page = totalPages;

        var products = await query
            .OrderBy(p => p.ProductName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var viewModel = new ViewModels.ProductIndexViewModel
        {
            Products = products,
            Categories = await _context.Categories.OrderBy(c => c.CategoryName).ToListAsync(),
            SearchTerm = search,
            CategoryId = categoryId,
            StockStatus = stockStatus,
            CurrentPage = page,
            TotalPages = totalPages,
            PageSize = pageSize,
            TotalCount = totalCount
        };

        return View(viewModel);
    }

    // GET: Products/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.ProductId == id);

        if (product == null) return NotFound();

        return View(product);
    }

    // GET: Products/Create
    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await _context.Categories.ToListAsync();
        return View();
    }

    // POST: Products/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(product);
        }

        _context.Add(product);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: Products/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        ViewBag.Categories = await _context.Categories.ToListAsync();
        return View(product);
    }

    // POST: Products/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Product product)
    {
        if (id != product.ProductId) return NotFound();

        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(product);
        }

        _context.Update(product);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: Products/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.ProductId == id);

        if (product == null) return NotFound();

        return View(product);
    }

    // POST: Products/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null) return RedirectToAction(nameof(Index));

        var hasPurchases = await _context.PurchaseItems.AnyAsync(pi => pi.ProductId == id);
        var hasSales = await _context.SaleItems.AnyAsync(si => si.ProductId == id);

        if (hasPurchases || hasSales)
        {
            TempData["Error"] = "Cannot delete this product because it has related purchases or sales records.";
            return RedirectToAction(nameof(Delete), new { id });
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}