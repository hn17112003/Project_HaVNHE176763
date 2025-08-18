using FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Pages.View
{
    public class CreateModel : PageModel
    {
		private readonly FinalProject.Models.ElectronicsStoreContext _context;

		public CreateModel(FinalProject.Models.ElectronicsStoreContext context)
		{
			_context = context;
		}
		public Product Product { get; set; }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            string role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(role) || !role.Equals("staff"))
            {
                return RedirectToPage("/View/Login");
            }
            return Page();
        }
        public async Task<IActionResult> OnPost(Product Product)
		{
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            {
                Product.UserId = userId;
            }
			await _context.Products.AddAsync(Product);
			await _context.SaveChangesAsync();
			return RedirectToPage("ManageProduct");
		}
    }
}
