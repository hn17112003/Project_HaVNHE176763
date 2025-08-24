using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FinalProject.Models;

namespace FinalProject.Pages.View
{
    public class CreateSupplierModel : PageModel
    {
        private readonly ElectronicsStoreContext _context;

        public CreateSupplierModel(ElectronicsStoreContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Supplier Supplier { get; set; } = default!;

        public IActionResult OnGet()
        {
            Supplier = new Supplier();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Suppliers.Add(Supplier);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Supplier '{Supplier.SupplierName}' has been created successfully.";
            return RedirectToPage("./ManageSuppliers");
        }
    }
}
