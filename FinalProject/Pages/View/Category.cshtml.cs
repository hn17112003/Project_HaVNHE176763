using FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Pages.View
{
    public class CategoryModel : PageModel
    {
        private readonly ElectronicsStoreContext _context;

        public CategoryModel(ElectronicsStoreContext context)
        {
            _context = context;
        }

        public IList<Product> Product { get; set; } = default!;
        public IList<Category> Category { get; set; } = default!;
        public string searchString { get; set; }

        public async Task<IActionResult> OnGetAsync(string searchTerm, int? categoryId, string? id)
        {
            searchString = searchTerm;

            // Get all categories
            Category = await _context.Categories.ToListAsync();

            // Filter products by search term if provided, and only show products with quantity > 0
            var productQuery = _context.Products
                .Include(x => x.Category)
                .Where(p => p.StockQuantity > 0) // Only include products with quantity > 0
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                productQuery = productQuery.Where(p => p.ProductName.Contains(searchString));
            }

            // Filter products by selected category if provided
            if (categoryId.HasValue)
            {
                productQuery = productQuery.Where(p => p.CategoryId == categoryId.Value);
            }

            // Fetch all products without pagination, filtered by quantity > 0
            Product = await productQuery.ToListAsync();

            // Handle add-to-cart if an ID is provided
            if (!string.IsNullOrEmpty(id))
            {
                int productId = int.Parse(id);
                int? userId = HttpContext.Session.GetInt32("UserId");

                // Check if UserId is null, meaning the user is not logged in
                if (userId == null)
                {
                    return RedirectToPage("/View/Login");
                }

                // Fetch the product to check its stock quantity
                var product = await _context.Products.FirstOrDefaultAsync(x => x.ProductId == productId);
                if (product == null)
                {
                    return NotFound();  // Product not found
                }

                // Check if the product is in stock
                if (product.StockQuantity <= 0)
                {
                    TempData["ErrorMessage"] = "This product is currently out of stock.";
                    return RedirectToPage();
                }

                // Check if the user has an existing shopping cart
                var shoppingCart = await _context.ShoppingCarts
                    .Include(sc => sc.CartItems)
                    .FirstOrDefaultAsync(sc => sc.UserId == userId);

                if (shoppingCart == null)
                {
                    // Create a new cart for the user if none exists
                    shoppingCart = new ShoppingCart
                    {
                        UserId = userId,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    _context.ShoppingCarts.Add(shoppingCart);
                    await _context.SaveChangesAsync();  // Save to generate CartId for new cart
                }

                // Check if the item already exists in the cart
                var cartItem = shoppingCart.CartItems
                    .FirstOrDefault(ci => ci.ProductId == productId);

                if (cartItem != null)
                {
                    // If the item is already in the cart, increase the quantity
                    cartItem.Quantity++;
                }
                else
                {
                    // Otherwise, add a new item to the cart
                    cartItem = new CartItem
                    {
                        CartId = shoppingCart.CartId,
                        ProductId = productId,
                        Quantity = 1
                    };
                    _context.CartItems.Add(cartItem);
                }

                // Update the cart's last updated timestamp
                shoppingCart.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
            }
            return Page();
        }
    }
}