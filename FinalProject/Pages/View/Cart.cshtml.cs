using FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Pages.View
{
    public class CartModel : PageModel
    {
        private readonly ElectronicsStoreContext _context;

        public CartModel(ElectronicsStoreContext context)
        {
            _context = context;
        }

        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public decimal Subtotal { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Retrieve Username and UserId from the session
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId != null && userId >= 0)
            {
                // Retrieve the ShoppingCart for the user
                var shoppingCart = await _context.ShoppingCarts
                    .Include(sc => sc.CartItems)
                    .ThenInclude(ci => ci.Product)
                    .FirstOrDefaultAsync(sc => sc.UserId == userId);

                if (shoppingCart != null)
                {
                    // Assign the retrieved CartItems
                    CartItems = shoppingCart.CartItems.ToList();

                    // Calculate the subtotal
                    Subtotal = CartItems.Sum(ci => ci.Product.Price * ci.Quantity ?? 0);
                }
            }
            else
            {
                return RedirectToPage("/View/Login");
            }
            return Page();
        }
    }
}
