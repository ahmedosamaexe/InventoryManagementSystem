using System;
using System.Linq;
using System.Threading.Tasks;
using InventoryManagementSystem.Data;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Controllers
{
    public class PurchasesController : Controller
    {
        private readonly AppDbContext _context;

        public PurchasesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Purchases
        public async Task<IActionResult> Index()
        {
            var purchases = await _context.Purchases
                .Include(p => p.Supplier)
                .OrderByDescending(p => p.PurchaseDate)
                .ToListAsync();

            return View(purchases);
        }

        // GET: Purchases/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var purchase = await _context.Purchases
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseItems)
                    .ThenInclude(pi => pi.Product)
                .FirstOrDefaultAsync(p => p.PurchaseId == id);

            if (purchase == null) return NotFound();

            return View(purchase);
        }

        // GET: Purchases/Create
        public async Task<IActionResult> Create()
        {
            await PopulateDropdownsAsync();
            return View(new PurchaseCreateViewModel());
        }

        // POST: Purchases/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PurchaseCreateViewModel model)
        {
            if (model.Items == null || !model.Items.Any())
                ModelState.AddModelError("", "Add at least one product to the purchase.");

            var supplierExists = await _context.Suppliers.AnyAsync(s => s.SupplierId == model.SupplierId);
            if (!supplierExists)
                ModelState.AddModelError(nameof(model.SupplierId), "Invalid supplier.");

            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync();
                return View(model);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var purchase = new Purchase
                {
                    SupplierId = model.SupplierId,
                    PurchaseDate = DateTime.Now
                };

                decimal total = 0;

                foreach (var itemInput in model.Items)
                {
                    if (itemInput.Quantity <= 0) continue;

                    var product = await _context.Products.FindAsync(itemInput.ProductId);
                    if (product == null) continue;

                    var item = new PurchaseItem
                    {
                        ProductId = itemInput.ProductId,
                        Quantity = itemInput.Quantity,
                        UnitCost = itemInput.UnitCost
                    };

                    total += item.Quantity * item.UnitCost;
                    purchase.PurchaseItems.Add(item);

                    // Stock increases automatically when a purchase is made
                    product.StockQuantity += itemInput.Quantity;
                }

                purchase.TotalAmount = total;

                _context.Purchases.Add(purchase);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return RedirectToAction(nameof(Details), new { id = purchase.PurchaseId });
            }
            catch
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError("", "Something went wrong while saving the purchase.");
                await PopulateDropdownsAsync();
                return View(model);
            }
        }

        private async Task PopulateDropdownsAsync()
        {
            ViewBag.Suppliers = await _context.Suppliers
                .OrderBy(s => s.SupplierName)
                .ToListAsync();

            ViewBag.Products = await _context.Products
                .OrderBy(p => p.ProductName)
                .ToListAsync();
        }
    }
}
