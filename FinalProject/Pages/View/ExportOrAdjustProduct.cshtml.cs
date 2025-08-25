using FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Pages.View
{
    public class ExportOrAdjustProductModel : PageModel
    {
        private readonly ElectronicsStoreContext _context;

        public ExportOrAdjustProductModel(ElectronicsStoreContext context)
        {
            _context = context;
        }

        [BindProperty]
        public int ProductId { get; set; }

        [BindProperty]
        public int Quantity { get; set; } 

        [BindProperty]
        public string Notes { get; set; } = string.Empty;

        [BindProperty]
        public int NewQuantity { get; set; }

        public Product? Product { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id);
            if (Product == null)
            {
                return NotFound();
            }

            ProductId = Product.ProductId;
            return Page();
        }

        // ===== Xuất kho =====
        public async Task<IActionResult> OnPostExportAsync()
        {
            var product = await _context.Products.FindAsync(ProductId);
            if (product == null)
                return NotFound();

            if (Quantity <= 0 || Quantity > product.StockQuantity)
            {
                ModelState.AddModelError("Quantity", "Invalid export quantity");
                Product = product;
                return Page();
            }

            product.StockQuantity -= Quantity;

            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToPage("/View/Login");

            var transaction = new InventoryTransaction
            {
                ProductId = product.ProductId,
                UserId = userId.Value,
                TransactionType = "Issue",
                QuantityChange = -Quantity,
                TransactionDate = DateTime.Now,
                Notes = Notes
            };

            _context.InventoryTransactions.Add(transaction);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Export Successfully!";
            return RedirectToPage("ManageProduct");
        }

        // ===== Điều chỉnh kho =====
        public async Task<IActionResult> OnPostAdjustAsync()
        {
            var product = await _context.Products.FindAsync(ProductId);
            if (product == null)
                return NotFound();

            int oldQuantity = product.StockQuantity ?? 0;
            int diff = NewQuantity - oldQuantity;

            if (diff == 0)
            {
                ModelState.AddModelError("NewQuantity", "Quantity unchanged, no adjustment required.");
                Product = product;
                return Page();
            }

            product.StockQuantity = NewQuantity;

            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToPage("/View/Login");

            var transaction = new InventoryTransaction
            {
                ProductId = product.ProductId,
                UserId = userId.Value,
                TransactionType = "Adjustment",
                QuantityChange = diff,
                TransactionDate = DateTime.Now,
                Notes = $"Adjust inventory: to {oldQuantity} -> {NewQuantity}"
            };

            _context.InventoryTransactions.Add(transaction);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Successful stock adjustment!";
            return RedirectToPage("ManageProduct");
        }
    }
}
