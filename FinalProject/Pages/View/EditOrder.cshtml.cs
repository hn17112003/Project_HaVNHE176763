using FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Pages.View
{
    public class EditOrderModel : PageModel
    {
        private readonly ElectronicsStoreContext _context;

        public EditOrderModel(ElectronicsStoreContext context)
        {
            _context = context;
        }
        [BindProperty]
        public Order Order { get; set; }
        public IList<User> User { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            string role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(role) || !role.Equals("staff"))
            {
                return RedirectToPage("/View/Login");
            }
			Order = await _context.Orders.Include(o => o.User).FirstOrDefaultAsync(o => o.OrderId == id);

			return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            var existingOrder = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.OrderId == Order.OrderId);

            if (existingOrder == null)
            {
                return NotFound();
            }

            // Store old statuses
            string oldPaymentStatus = existingOrder.PaymentStatus?.ToLower();
            string oldShippingStatus = existingOrder.ShippingStatus?.ToLower();

            string newPaymentStatus = Order.PaymentStatus?.ToLower();
            string newShippingStatus = Order.ShippingStatus?.ToLower();

            // Deduct stock only when order changes to "delivered" and is "paid"
            if (oldShippingStatus != "delivered" && newShippingStatus == "delivered" && newPaymentStatus == "paid")
            {
                foreach (var item in existingOrder.OrderDetails)
                {
                    if (item.Product != null)
                    {
                        item.Product.StockQuantity -= item.Quantity;
                        if (item.Product.StockQuantity < 0)
                        {
                            item.Product.StockQuantity = 0; // Prevent negative stock
                        }
                    }
                }
            }

            // Update order information
            existingOrder.Status = Order.Status;
            existingOrder.ShippingAddress = Order.ShippingAddress;
            existingOrder.PaymentStatus = Order.PaymentStatus;
            existingOrder.ShippingStatus = Order.ShippingStatus;
            existingOrder.OrderDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return RedirectToPage("ManageOrder");
        }




    }
}
