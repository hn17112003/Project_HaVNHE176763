using FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Pages.View
{
    public class ManageAccountModel : PageModel
    {
        private readonly ElectronicsStoreContext _context;

        public ManageAccountModel(ElectronicsStoreContext context)
        {
            _context = context;
        }
        public IList<User> Users { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            string role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(role) || !role.Equals("admin"))
            {
                return RedirectToPage("/View/Login");
            }
            Users = await _context.Users.ToListAsync();
            return Page();
        }
    }
}
