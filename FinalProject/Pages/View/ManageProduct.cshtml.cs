using FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Pages.View
{
    public class ManageProductModel : PageModel
    {
        private readonly ElectronicsStoreContext _context;

        public ManageProductModel(ElectronicsStoreContext context)
        {
            _context = context;
        }
        public IList<Product> Products { get; set; }
        public IList<Category> Category { get; set; } = default!;


        public async Task<IActionResult> OnGetAsync()
        {
            string role = HttpContext.Session.GetString("Role");
                if (string.IsNullOrEmpty(role)||!role.Equals("staff"))
                {
                    return RedirectToPage("/View/Login");
                }
            Products = await _context.Products.Include(x=>x.Category).ToListAsync();
            return Page();
        }
    }
}
