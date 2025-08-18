using FinalProject.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace FinalProject.Pages.View
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string LoginMessage { get; set; }
        public List<User> Users { get; set; }

        public IActionResult OnGet()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            string username = HttpContext.Session.GetString("Username");
            if ((userId != null && userId >= 0) || !string.IsNullOrEmpty(username))
            {
                return RedirectToPage("/View/Category");
            }

            Users = ElectronicsStoreContext.Ins.Users
                .Include(u => u.OrderStaffs)
                .Include(u => u.ShoppingCarts)
                .ToList();

            return Page();
        }
        public IActionResult OnPost(string username, string password)
        {
            // Step 1: Trim the inputs
            Username = username.Trim();
            Password = password.Trim();

            var user = ElectronicsStoreContext.Ins.Users
                        .SingleOrDefault(u => u.Username == Username);

            if (user != null)
            {
                //bool isPasswordValid = BCrypt.Net.BCrypt.Verify(Password, user.Password);
                bool isPasswordValid = true;
                if (user.Password != Password)
                {
                    isPasswordValid = false;
                }

                if (isPasswordValid)
                {
                    string redirectUrl = "/";
                    if (user.Role.Equals("admin"))
                    {
                        redirectUrl += "View/ManageAccount";
                    } else if (user.Role.Equals("staff"))
                    {
                        redirectUrl += "View/ManageProduct";
                    } else
                    {
                        redirectUrl += "View/Category";
                    }
                    HttpContext.Session.SetString("Username", user.Username);
                    HttpContext.Session.SetString("Role", user.Role);
                    HttpContext.Session.SetInt32("UserId", user.UserId);
                    return RedirectToPage(redirectUrl);

                }
                else
                {
                    LoginMessage = "Username or password is incorrect.";
                }
            }
            else
            {
                LoginMessage = "Username or password is incorrect.";
            }

            return Page();
        }
        public async Task<IActionResult> OnGetLogin()
        {
            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = Url.Page("/View/Login", "GoogleResponse")
            };

            return Challenge(authenticationProperties, GoogleDefaults.AuthenticationScheme);
        }
        public async Task<IActionResult> OnGetGoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (result?.Principal == null)
            {
                return RedirectToPage("/View/Login");
            }

            var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = result.Principal.FindFirst(ClaimTypes.Name)?.Value;

            var user = ElectronicsStoreContext.Ins.Users.SingleOrDefault(u => u.Email == email);

            if (user != null)
            {
                string redirectUrl = "/";
                if (user.Role.Equals("admin"))
                {
                    redirectUrl += "View/ManageAccount";
                }
                else if (user.Role.Equals("staff"))
                {
                    redirectUrl += "View/ManageProduct";
                }
                else
                {
                    redirectUrl += "View/Category";
                }
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("Role", user.Role);
                HttpContext.Session.SetInt32("UserId", user.UserId);
                return RedirectToPage(redirectUrl);
            }
            else
            {
                return RedirectToPage("/View/Register", new { email = email, name = name });
            }
        }

    }
}
