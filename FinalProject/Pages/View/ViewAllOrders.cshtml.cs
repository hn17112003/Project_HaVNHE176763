using FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Pages.View
{
    public class ViewAllOrdersModel : PageModel
    {
        private readonly ElectronicsStoreContext _context;

        public ViewAllOrdersModel(ElectronicsStoreContext context)
        {
            _context = context;
        }

        public IList<Order> Orders { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            string role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(role) || !role.Equals("admin"))
            {
                return RedirectToPage("/View/Login");
            }

            Orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Staff)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return Page();
        }
    }
}
