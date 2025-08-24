using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FinalProject.Models;

namespace FinalProject.Pages.View
{
    public class ViewSupplierModel : PageModel
    {
        private readonly ElectronicsStoreContext _context;

        public ViewSupplierModel(ElectronicsStoreContext context)
        {
            _context = context;
        }

        public Supplier Supplier { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _context.Suppliers
                .Include(s => s.GoodsReceipts)
                .FirstOrDefaultAsync(m => m.SupplierId == id);
            if (supplier == null)
            {
                return NotFound();
            }

            Supplier = supplier;
            return Page();
        }
    }
}
