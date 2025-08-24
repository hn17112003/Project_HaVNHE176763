using FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Xml.Linq;

namespace FinalProject.Pages.View
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Name { get; set; }
        public string Message { get; set; }

        public string Phone { get; set; }
		public IActionResult OnGet(string email, string name)
        {
            Email = email;
            Name = name;
            int? userId = HttpContext.Session.GetInt32("UserId");
            string username = HttpContext.Session.GetString("Username");
            if ((userId != null && userId >= 0) || !string.IsNullOrEmpty(username))
            {
                return RedirectToPage("/View/Category");
            }
            return Page();
        }

        public IActionResult OnPost(User user, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Email) ||
                string.IsNullOrWhiteSpace(user.Password) || string.IsNullOrWhiteSpace(confirmPassword) ||
                string.IsNullOrWhiteSpace(user.Phone))
            {
                Message = "All fields are required.";
                return Page();
            }

            if (!user.Email.Contains("@"))
            {
                Message = "Please enter a valid email address.";
                return Page();
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(user.Phone, @"^[0-9]{9,11}$"))
            {
                Message = "Please enter a valid phone number (9-11 digits).";
                return Page();
            }

            if (user.Password != confirmPassword)
            {
                Message = "Password and Confirm Password do not match.";
                return Page();
            }

            var existingUser = ElectronicsStoreContext.Ins.Users
                .FirstOrDefault(u => u.Username == user.Username || u.Email == user.Email || u.Phone == user.Phone);

            if (existingUser != null)
            {
                Message = "Username or Email already exists. Please use another one.";
                return Page();
            }

            //user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            user.CreatedAt = DateTime.Now;
            user.UpdatedAt = DateTime.Now;

            ElectronicsStoreContext.Ins.Users.Add(user);
            ElectronicsStoreContext.Ins.SaveChanges();

            return RedirectToPage("/View/Login");
        }
    }
}
