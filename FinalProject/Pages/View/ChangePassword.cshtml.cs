using FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinalProject.Pages.View
{
    public class ChangePasswordModel : PageModel
    {
        private readonly ElectronicsStoreContext _context;
        public ChangePasswordModel(ElectronicsStoreContext context)
        {
            _context = context;
        }
        public string Email { get; set; }
        public string Error { get; set; }
        public IActionResult OnGet(string email)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            {
                User user = _context.Users.Where(x=>x.UserId== userId).FirstOrDefault();
                Email = user.Email;
            }else if(userId == null && !string.IsNullOrEmpty(email))
            {
                if (CheckEmail(email))
                {
                    Email = email;
                }
                else
                {
                    return Redirect("/View/VerifyEmail");
                }
            }else if (userId == null && string.IsNullOrEmpty(email))
            {
                return Redirect("/View/VerifyEmail");
            }
            return Page();
        }
        public IActionResult OnPost(string newPassword, string email)
        {
            User user = _context.Users.Where(x => x.Email.Equals(email)).FirstOrDefault();
            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            _context.SaveChanges();

            return RedirectToPage("/View/Logout");
        }
        private bool CheckEmail(string email)
        {
            return _context.Users.Any(u => u.Email == email);
        }
    }
}
