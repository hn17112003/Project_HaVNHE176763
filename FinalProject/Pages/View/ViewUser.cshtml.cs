using FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Pages.View
{
    public class ViewUserModel : PageModel
    {
        private readonly ElectronicsStoreContext _context;

        public ViewUserModel(ElectronicsStoreContext context)
        {
            _context = context;
        }
        public User User { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            User = await _context.Users.FindAsync(userId);
            return Page();
        }
        }
    }
