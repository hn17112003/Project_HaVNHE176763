using FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinalProject.Pages.View
{
    public class VerifyEmailModel : PageModel
    {
        private readonly ElectronicsStoreContext _context;
        public string error { get; set; }
        public VerifyEmailModel(ElectronicsStoreContext context)
        {
            _context = context;
        }
        public IActionResult OnGet()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            {
                return Redirect("/View/ChangePassword");
            }
            return Page();
        }
        public IActionResult OnPost(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                error = "Email is required";
            }
            else
            {
                if (CheckEmail(email))
                {
                    return Redirect("/View/ChangePassword?email="+email);
                }
                error = "Something went wrong";
            }
            return Page();
        }
        private bool CheckEmail(string email)
        {
            return _context.Users.Any(u => u.Email == email);
        }
    }
}
