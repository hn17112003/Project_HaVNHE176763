using FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Pages.View
{
    public class InventoryHistoryModel : PageModel
    {
        private readonly ElectronicsStoreContext _context;

        public InventoryHistoryModel(ElectronicsStoreContext context)
        {
            _context = context;
        }

        public IList<InventoryTransaction> Transactions { get; set; } = new List<InventoryTransaction>();

        [BindProperty(SupportsGet = true)]
        public string? SearchProduct { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? TransactionType { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? FromDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? ToDate { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            string role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(role) || (!role.Equals("staff") && !role.Equals("admin")))
            {
                return RedirectToPage("/View/Login");
            }

            var query = _context.InventoryTransactions
                        .Include(t => t.Product)
                        .Include(t => t.User)
                        .AsQueryable();

            if (!string.IsNullOrEmpty(SearchProduct))
            {
                query = query.Where(t => t.Product.ProductName.Contains(SearchProduct));
            }

            if (!string.IsNullOrEmpty(TransactionType))
            {
                query = query.Where(t => t.TransactionType == TransactionType);
            }

            if (FromDate.HasValue)
            {
                query = query.Where(t => t.TransactionDate >= FromDate.Value);
            }

            if (ToDate.HasValue)
            {
                query = query.Where(t => t.TransactionDate <= ToDate.Value);
            }

            Transactions = await query
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();

            return Page();
        }
    }
}
