using FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FinalProject.Pages.View
{
    public class ManageUserModel : PageModel
    {
        private readonly ElectronicsStoreContext _context;

        public ManageUserModel(ElectronicsStoreContext context)
        {
            _context = context;
        }
        [BindProperty]
        public User User { get; set; }
        public int userEditId { get; set; }
        public List<SelectListItem> Roles { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            //userEditId = id;
            int? userId = HttpContext.Session.GetInt32("UserId");
            User = await _context.Users.FindAsync(userId);
            
            if (User == null)
            {
                return Redirect("/View/Login");
            }
            userId = User.UserId;
            return Page();
        }
        public async Task<IActionResult> OnPost(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            User userToUpdate = await _context.Users.FindAsync(userId);
            if (userToUpdate == null)
            {
                return Redirect("/View/Login");
            }
            userToUpdate.Address = User.Address;
            userToUpdate.Phone = User.Phone;
     

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
             
                ModelState.AddModelError(string.Empty, "Error updating the user. Please try again.");
                return Page();
            }

            return Page();
        }
    }
}
