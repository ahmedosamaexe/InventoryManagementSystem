using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.ViewModels
{
    public class PurchaseCreateViewModel
    {
        [Required(ErrorMessage = "Please select a supplier.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid supplier.")]
        [Display(Name = "Supplier")]
        public int SupplierId { get; set; }

        public List<PurchaseItemInput> Items { get; set; } = new();
    }

    public class PurchaseItemInput
    {
        [Required(ErrorMessage = "Please select a product.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid product.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Unit cost is required.")]
        [Range(0.01, 9999999.99, ErrorMessage = "Unit cost must be between 0.01 and 9,999,999.99.")]
        public decimal UnitCost { get; set; }
    }
}
