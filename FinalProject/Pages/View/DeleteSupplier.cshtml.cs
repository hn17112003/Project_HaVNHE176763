using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FinalProject.Models;

namespace FinalProject.Pages.View
{
    public class DeleteSupplierModel : PageModel
    {
        private readonly ElectronicsStoreContext _context;

        public DeleteSupplierModel(ElectronicsStoreContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Supplier Supplier { get; set; } = default!;
        public int GoodsReceiptCount { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _context.Suppliers.FirstOrDefaultAsync(m => m.SupplierId == id);
            if (supplier == null)
            {
                return NotFound();
            }

            Supplier = supplier;

            // Count related goods receipts
            GoodsReceiptCount = await _context.GoodsReceipts
                .CountAsync(gr => gr.SupplierId == id);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }

            // Check if supplier has related goods receipts
            var hasGoodsReceipts = await _context.GoodsReceipts
                .AnyAsync(gr => gr.SupplierId == id);

            if (hasGoodsReceipts)
            {
                TempData["ErrorMessage"] = $"Cannot delete supplier '{supplier.SupplierName}' because it has related goods receipts.";
                return RedirectToPage("./ManageSuppliers");
            }

            // Delete the supplier
            _context.Suppliers.Remove(supplier);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Supplier '{supplier.SupplierName}' has been deleted successfully.";
            return RedirectToPage("./ManageSuppliers");
        }
    }
}
