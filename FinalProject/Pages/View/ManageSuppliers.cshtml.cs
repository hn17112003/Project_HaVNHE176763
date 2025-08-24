using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FinalProject.Models;

namespace FinalProject.Pages.View
{
    public class ManageSuppliersModel : PageModel
    {
        private readonly ElectronicsStoreContext _context;

        public ManageSuppliersModel(ElectronicsStoreContext context)
        {
            _context = context;
        }

        public IList<Supplier> Suppliers { get; set; } = default!;

        // Filter properties
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        // Statistics properties
        public int TotalSuppliers { get; set; }

        public async Task OnGetAsync()
        {
            // Build query with filters
            var query = _context.Suppliers.AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                query = query.Where(s => 
                    s.SupplierName.Contains(SearchTerm) ||
                    (s.ContactPerson != null && s.ContactPerson.Contains(SearchTerm)) ||
                    (s.Email != null && s.Email.Contains(SearchTerm)));
            }

            // Execute query and order by name
            Suppliers = await query
                .OrderBy(s => s.SupplierName)
                .ToListAsync();

            // Calculate statistics
            TotalSuppliers = await _context.Suppliers.CountAsync();
        }
    }
}
