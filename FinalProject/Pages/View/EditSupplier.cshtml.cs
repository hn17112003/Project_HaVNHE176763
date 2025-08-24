using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FinalProject.Models;

namespace FinalProject.Pages.View
{
    public class EditSupplierModel : PageModel
    {
        private readonly ElectronicsStoreContext _context;

        public EditSupplierModel(ElectronicsStoreContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Supplier Supplier { get; set; } = default!;

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
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Supplier).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SupplierExists(Supplier.SupplierId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            TempData["SuccessMessage"] = $"Supplier '{Supplier.SupplierName}' has been updated successfully.";
            return RedirectToPage("./ManageSuppliers");
        }

        private bool SupplierExists(int id)
        {
            return _context.Suppliers.Any(e => e.SupplierId == id);
        }
    }
}
