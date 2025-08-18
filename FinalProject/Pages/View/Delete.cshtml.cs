using FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Pages.View
{
    public class DeleteModel : PageModel
    {
		private readonly FinalProject.Models.ElectronicsStoreContext _context;

		public DeleteModel(FinalProject.Models.ElectronicsStoreContext context)
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
        public async Task<IActionResult> OnPost(Product Product)
		{
			var productfromdb = _context.Products.Find(Product.ProductId);
			 _context.Products.Remove(productfromdb);
			await _context.SaveChangesAsync();
			return RedirectToPage("ManageProduct");
		}
    }
}
