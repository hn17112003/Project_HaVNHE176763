using FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Pages.View
{
    public class EditModel : PageModel
    {
		private readonly ElectronicsStoreContext _context;

		public EditModel(ElectronicsStoreContext context)
		{
			_context = context;
		}
		public Product Product { get; set; }
        public IList<Category> Category { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            string role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(role) || !role.Equals("staff"))
            {
                return RedirectToPage("/View/Login");
            }
            Product = _context.Products.Find(id);
            Category = await _context.Categories.ToListAsync();
            return Page();
        }
		public async Task<IActionResult> OnPost(Product product)
		{
			product.UpdatedAt = DateTime.Now;
			_context.Products.Update(product);
			await _context.SaveChangesAsync();
			return RedirectToPage("ManageProduct");
		}
	}
}
