using System.Net.ServerSentEvents;

namespace InventoryManagementSystem.Models;

public class Sale
{
    public int SaleId { get; set; }

    public DateTime SaleDate { get; set; } = DateTime.Now;

    public decimal TotalAmount { get; set; }

    [MaxLength(150)]
    public string? CustomerInfo { get; set; }

    // Navigation
    public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
}