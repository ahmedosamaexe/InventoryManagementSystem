using System.Collections.Generic;

namespace InventoryManagementSystem.ViewModels
{
    public class PurchaseCreateViewModel
    {
        public int SupplierId { get; set; }
        public List<PurchaseItemInput> Items { get; set; } = new();
    }

    public class PurchaseItemInput
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
    }
}
