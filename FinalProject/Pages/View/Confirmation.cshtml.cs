using FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Pages.View
{
    public class ConfirmationModel : PageModel
    {
        private readonly ElectronicsStoreContext _context;
        public int PendingOrderCount { get; set; }
        public int DeliveredOrderCount { get; set; }

        public ConfirmationModel(ElectronicsStoreContext context)
        {
            _context = context;
        }

        public List<Order> Orders { get; set; } = new List<Order>();
        public List<string> ShippingStatuses { get; set; } = new List<string>();
        public List<string> Statuses { get; set; } = new List<string> { "return", "returnprocessing", "pending", "completed" };

        [BindProperty(SupportsGet = true)]
        public string SelectedShippingStatus { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SelectedStatus { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToPage("/View/Login");
            }

            PendingOrderCount = await _context.Orders
    .CountAsync(o => o.UserId == userId && o.ShippingStatus == "pending");

            DeliveredOrderCount = await _context.Orders
                .CountAsync(o => o.UserId == userId && o.ShippingStatus == "delivered");

            ShippingStatuses = await _context.Orders
                .Where(o => o.UserId == userId && o.ShippingStatus != null)
                .Select(o => o.ShippingStatus)
                .Distinct()
                .ToListAsync();

            var ordersQuery = _context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .AsQueryable();

            if (!string.IsNullOrEmpty(SelectedStatus))
            {
                ordersQuery = ordersQuery.Where(o => o.Status == SelectedStatus);
            }

            if (!string.IsNullOrEmpty(SelectedShippingStatus))
            {
                ordersQuery = ordersQuery.Where(o => o.ShippingStatus == SelectedShippingStatus);
            }

            Orders = await ordersQuery.ToListAsync();

            return Page();
        }

        public IActionResult OnPost(int OrderId)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToPage("/View/Login");
            }

            var order = _context.Orders.FirstOrDefault(o => o.OrderId == OrderId && o.UserId == userId);
            if (order == null)
            {
                return NotFound();
            }

            if (order.ShippingStatus == "pending")
            {
                order.ShippingStatus = "cancelled";
                _context.SaveChanges();
            }
            else
            {
                TempData["ErrorMessage"] = "This order cannot be canceled as it is already being processed.";
            }

            return RedirectToPage("/View/Confirmation");
        }


    }
}
