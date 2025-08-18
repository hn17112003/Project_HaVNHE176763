using FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinalProject.Pages.View
{
    public class VerifyCurrentPasswordModel : PageModel
    {
        private readonly ElectronicsStoreContext _context;

        public VerifyCurrentPasswordModel(ElectronicsStoreContext context)
        {
            _context = context;
        }
        public string error {  get; set; }
        public void OnGet()
        {
            
        }
        public IActionResult OnPost(string currentPassword)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            {
                User currentUser = _context.Users.FirstOrDefault(x => x.UserId == userId);
                if (currentUser != null)
                {
                    bool isPasswordValid = BCrypt.Net.BCrypt.Verify(currentPassword, currentUser.Password);

                    if (isPasswordValid)
                    {
                        return RedirectToPage("/View/ChangePassword");
                    }
                    else
                    {
                        error = "Incorrect current password.";
                    }
                }
                else
                {
                    error = "Something went wrong!";
                }
            }
            else
            {
                return Redirect("/View/Login");
            }
            return Page();
        }
    }
}
