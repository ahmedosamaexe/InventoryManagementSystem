using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace InventoryManagementSystem.Controllers;

public class RagController : Controller
{
    private readonly AppDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public RagController(AppDbContext context, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public IActionResult Index()
    {
        return View(new RagViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Ask(RagViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Question))
        {
            model.Answer = "Please enter a question.";
            return View("Index", model);
        }

        var dataContext = await BuildDataContextAsync();
        model.Answer = await AskGeminiAsync(model.Question, dataContext);

        return View("Index", model);
    }

    private async Task<string> BuildDataContextAsync()
    {
        var products = await _context.Products
            .Include(p => p.Category)
            .ToListAsync();

        var totalStockValue = products.Sum(p => p.UnitPrice * p.StockQuantity);
        var lowStock = products.Where(p => p.StockQuantity > 0 && p.StockQuantity <= p.LowStockThreshold).ToList();
        var outOfStock = products.Where(p => p.StockQuantity == 0).ToList();

        var recentSales = await _context.Sales
            .Include(s => s.SaleItems)
            .OrderByDescending(s => s.SaleDate)
            .Take(20)
            .ToListAsync();

        var recentPurchases = await _context.Purchases
            .Include(p => p.Supplier)
            .OrderByDescending(p => p.PurchaseDate)
            .Take(20)
            .ToListAsync();

        var sb = new StringBuilder();

        sb.AppendLine($"Total products: {products.Count}");
        sb.AppendLine($"Total stock value: {totalStockValue:N2}");

        sb.AppendLine("\nAll products:");
        foreach (var p in products)
        {
            sb.AppendLine($"- {p.ProductName} | SKU: {p.SKU} | Category: {p.Category.CategoryName} | Price: {p.UnitPrice:N2} | Stock: {p.StockQuantity} | Low stock threshold: {p.LowStockThreshold}");
        }

        sb.AppendLine($"\nLow stock products ({lowStock.Count}): {string.Join(", ", lowStock.Select(p => p.ProductName))}");
        sb.AppendLine($"Out of stock products ({outOfStock.Count}): {string.Join(", ", outOfStock.Select(p => p.ProductName))}");

        sb.AppendLine($"\nRecent sales (most recent {recentSales.Count}):");
        foreach (var s in recentSales)
        {
            sb.AppendLine($"- {s.SaleDate:yyyy-MM-dd} | Total: {s.TotalAmount:N2} | Items sold: {s.SaleItems.Sum(i => i.Quantity)}");
        }

        sb.AppendLine($"\nRecent purchases (most recent {recentPurchases.Count}):");
        foreach (var p in recentPurchases)
        {
            sb.AppendLine($"- {p.PurchaseDate:yyyy-MM-dd} | Supplier: {p.Supplier.SupplierName} | Total: {p.TotalAmount:N2}");
        }

        return sb.ToString();
    }

    private async Task<string> AskGeminiAsync(string question, string dataContext)
    {
        var apiKey = _configuration["GeminiSettings:ApiKey"];

        if (string.IsNullOrWhiteSpace(apiKey) || apiKey == "PUT_YOUR_GEMINI_API_KEY_HERE")
        {
            return "Gemini API key is not configured. Add it in appsettings.json under GeminiSettings:ApiKey.";
        }

        var prompt = $@"You are an assistant for an inventory management system. Answer the question using ONLY the data below. If the answer isn't in the data, say you don't have that information. Keep the answer short and clear.

DATA:
{dataContext}

QUESTION:
{question}";

        var requestBody = new
        {
            contents = new[]
            {
                new { parts = new[] { new { text = prompt } } }
            }
        };

        var client = _httpClientFactory.CreateClient();
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3.6-flash:generateContent?key={apiKey}";        var response = await client.PostAsJsonAsync(url, requestBody);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            return $"Error {response.StatusCode}: {errorBody}";
        }

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();

        try
        {
            var text = json
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return text ?? "No answer received.";
        }
        catch
        {
            return "Sorry, I couldn't understand the assistant's response.";
        }
    }
}