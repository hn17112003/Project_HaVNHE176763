using FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Pages.View
{
    public class CheckoutModel : PageModel
    {
        private readonly ElectronicsStoreContext _context;

        public CheckoutModel(ElectronicsStoreContext context)
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
        public async Task<IActionResult> OnPostProceedToPaypalAsync()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToPage("/View/Login");
            }

            // 👉 Kiểm tra số điện thoại người dùng
            var user = await _context.Users.FindAsync(userId);
            if (string.IsNullOrWhiteSpace(user?.Phone))
            {
                TempData["ErrorMessage"] = "Please update your phone number in your profile before proceeding.";
                return RedirectToPage(); // Quay lại trang Checkout và hiển thị cảnh báo
            }

            // Retrieve cart items
            var shoppingCart = await _context.ShoppingCarts
                .Include(sc => sc.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(sc => sc.UserId == userId);

            if (shoppingCart == null || !shoppingCart.CartItems.Any())
            {
                return RedirectToPage("/View/Checkout");
            }

            // Tìm staff có ít đơn hàng nhất
            var staffWithLeastOrders = await _context.Users
                .Where(u => u.Role == "staff")
                .Select(u => new
                {
                    Staff = u,
                    OrderCount = _context.Orders.Count(o => o.StaffId == u.UserId)
                })
                .OrderBy(x => x.OrderCount)
                .FirstOrDefaultAsync();

            // Tạo đơn hàng
            var order = new Order
            {
                UserId = userId,
                TotalAmount = shoppingCart.CartItems.Sum(ci => ci.Product.Price * ci.Quantity ?? 0),
                Status = "Pending",
                OrderDate = DateTime.Now,
                ShippingAddress = "Dong Cam, Gia Phu, Lao Cai, Viet Nam",
                PaymentStatus = "unpaid",
                ShippingStatus = "pending",
                StaffId = staffWithLeastOrders?.Staff.UserId
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Chi tiết đơn hàng
            foreach (var cartItem in shoppingCart.CartItems)
            {
                _context.OrderDetails.Add(new OrderDetail
                {
                    OrderId = order.OrderId,
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    Price = cartItem.Product.Price
                });
            }

            // Giao hàng
            _context.Shippings.Add(new Shipping
            {
                OrderId = order.OrderId,
                TrackingNumber = "GeneratedTrackingNumber",
                ShippingDate = DateTime.Now,
                Status = "pending"
            });

            // Thanh toán
            _context.Payments.Add(new Payment
            {
                OrderId = order.OrderId,
                PaymentMethod = "PayPal",
                PaymentDate = DateTime.Now,
                Amount = order.TotalAmount,
                Status = "pending"
            });

            await _context.SaveChangesAsync();

            _context.CartItems.RemoveRange(shoppingCart.CartItems);
            await _context.SaveChangesAsync();

            return RedirectToPage("/View/Confirmation");
        }


    }
}
