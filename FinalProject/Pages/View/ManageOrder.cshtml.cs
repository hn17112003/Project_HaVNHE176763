using FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Pages.View
{
    public class ManageOrderModel : PageModel
    {
        private readonly ElectronicsStoreContext _context;

        public ManageOrderModel(ElectronicsStoreContext context)
        {
            _context = context;
        }
        public IList<Order> Orders { get; set; }
        public IList<User> Users { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            string role = HttpContext.Session.GetString("Role");
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (string.IsNullOrEmpty(role) || !role.Equals("staff") || userId == null)
            {
                return RedirectToPage("/View/Login");
            }

            Orders = await _context.Orders
                .Where(o => o.StaffId == userId) 
                .Include(o => o.User)
                .ToListAsync();

            return Page();
        }

    }
}
