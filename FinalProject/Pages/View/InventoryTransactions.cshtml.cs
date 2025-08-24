using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FinalProject.Models;

namespace FinalProject.Pages.View
{
    public class InventoryTransactionsModel : PageModel
    {
        private readonly ElectronicsStoreContext _context;

        public InventoryTransactionsModel(ElectronicsStoreContext context)
        {
            _context = context;
        }

        public IList<InventoryTransaction> Transactions { get; set; } = default!;
        public IList<Product> Products { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public int? ProductId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? TransactionType { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? FromDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? ToDate { get; set; }

        public async Task OnGetAsync()
        {
            // Load products for filter dropdown
            Products = await _context.Products
                .OrderBy(p => p.ProductName)
                .ToListAsync();

            // Build query with filters
            var query = _context.InventoryTransactions
                .Include(t => t.Product)
                .Include(t => t.User)
                .AsQueryable();

            // Apply filters
            if (ProductId.HasValue)
            {
                query = query.Where(t => t.ProductId == ProductId.Value);
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
                var endDate = ToDate.Value.AddDays(1); // Include the entire day
                query = query.Where(t => t.TransactionDate < endDate);
            }

            // Execute query and order by date descending
            Transactions = await query
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }
    }
}
