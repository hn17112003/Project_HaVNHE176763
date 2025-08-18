using FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Pages.View
{
    public class DeleteUserModel : PageModel
    {
        private readonly ElectronicsStoreContext _context;

        public DeleteUserModel(ElectronicsStoreContext context)
        {
            _context = context;
        }

        [BindProperty]
        public User User { get; set; }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            string role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(role) || !role.Equals("admin"))
            {
                return RedirectToPage("/View/Login");
            }
            User = await _context.Users.FindAsync(id);

            if (User == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (User == null)
            {
                return NotFound();
            }

            var userToDelete = await _context.Users.FindAsync(User.UserId);
            if (userToDelete == null)
            {
                return NotFound(); // Tr? v? 404 n?u ng??i dùng không t?n t?i
            }

            _context.Users.Remove(userToDelete);
            await _context.SaveChangesAsync();

            return RedirectToPage("ManageAccount");
        }
    }
}
