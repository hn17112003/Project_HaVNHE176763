using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FinalProject.Models;

namespace FinalProject.Pages.Reports
{
    public class TopBuyersModel : PageModel
    {
        private readonly ElectronicsStoreContext _context;

        public TopBuyersModel(ElectronicsStoreContext context)
        {
            _context = context;
        }

        public List<TopBuyerDto> TopBuyers { get; set; } = new();

        public void OnGet()
        {
            TopBuyers = _context.Users
                .Where(u => u.Role == "User")
                .Select(u => new TopBuyerDto
                {
                    Username = u.Username,
                    Email = u.Email,
                    TotalSpent = _context.Orders
                        .Where(o => o.UserId == u.UserId)
                        .Sum(o => (decimal?)o.TotalAmount) ?? 0
                })
                .OrderByDescending(u => u.TotalSpent)
                .Take(5)
                .ToList();
        }


    }

    public class TopBuyerDto
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public decimal TotalSpent { get; set; }
    }
}
