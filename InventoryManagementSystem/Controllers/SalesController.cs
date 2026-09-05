using System.Text.Json;

namespace InventoryManagementSystem.Controllers;

public class SalesController : Controller
{
    private readonly AppDbContext _context;
    private readonly StockService _stockService;

    public SalesController(AppDbContext context, StockService stockService)
    {
        _context = context;
        _stockService = stockService;
    }

    // GET: Sales
    public async Task<IActionResult> Index()
    {
        var sales = await _context.Sales
            .Include(s => s.SaleItems)
            .OrderByDescending(s => s.SaleDate)
            .ToListAsync();

        return View(sales);
    }

    // GET: Sales/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var sale = await _context.Sales
            .Include(s => s.SaleItems)
                .ThenInclude(si => si.Product)
            .FirstOrDefaultAsync(s => s.SaleId == id);

        if (sale == null) return NotFound();

        return View(sale);
    }

    // GET: Sales/Customers
    public async Task<IActionResult> Customers()
    {
        var customers = await _context.Sales
            .Where(s => !string.IsNullOrEmpty(s.CustomerInfo))
            .GroupBy(s => s.CustomerInfo)
            .Select(g => new
            {
                CustomerInfo = g.Key,
                TotalOrders = g.Count(),
                TotalSpent = g.Sum(s => s.TotalAmount),
                LastPurchaseDate = g.Max(s => s.SaleDate)
            })
            .OrderByDescending(c => c.LastPurchaseDate)
            .ToListAsync();

        return View(customers);
    }

    // GET: Sales/Create
    public async Task<IActionResult> Create()
    {
        await PopulateProductsAsync();
        return View();
    }

    // POST: Sales/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string? customerInfo, List<int>? productIds, List<int>? quantities)
    {
        var items = new List<(int ProductId, int Quantity)>();

        if (productIds != null && quantities != null)
        {
            for (int i = 0; i < Math.Min(productIds.Count, quantities.Count); i++)
            {
                if (productIds[i] > 0 && quantities[i] > 0)
                    items.Add((productIds[i], quantities[i]));
            }
        }

        if (!items.Any())
        {
            ModelState.AddModelError("", "Add at least one product to the sale.");
            await PopulateProductsAsync();
            ViewBag.CustomerInfo = customerInfo;
            return View();
        }

        var sale = new Sale { CustomerInfo = customerInfo };

        foreach (var (productId, quantity) in items)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                ModelState.AddModelError("", "One of the selected products could not be found.");
                await PopulateProductsAsync();
                ViewBag.CustomerInfo = customerInfo;
                return View();
            }

            sale.SaleItems.Add(new SaleItem
            {
                ProductId = product.ProductId,
                Quantity = quantity,
                UnitPrice = product.UnitPrice
            });
        }

        try
        {
            _stockService.ProcessSale(sale);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            await PopulateProductsAsync();
            ViewBag.CustomerInfo = customerInfo;
            return View();
        }

        _context.Sales.Add(sale);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Sale recorded successfully.";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateProductsAsync()
    {
        var products = await _context.Products
            .OrderBy(p => p.ProductName)
            .ToListAsync();

        var productsForJs = products.Select(p => new
        {
            id = p.ProductId,
            name = p.ProductName,
            sku = p.SKU,
            price = p.UnitPrice,
            stock = p.StockQuantity
        });

        ViewBag.Products = products;
        ViewBag.ProductsJson = JsonSerializer.Serialize(productsForJs);
    }
}