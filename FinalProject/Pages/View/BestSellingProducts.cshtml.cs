using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FinalProject.Models;

namespace FinalProject.Pages.Reports
{
    public class BestSellingProductsModel : PageModel
    {
        private readonly ElectronicsStoreContext _context;

        public BestSellingProductsModel(ElectronicsStoreContext context)
        {
            _context = context;
        }

        public List<BestSellingProductDto> BestSellingProducts { get; set; } = new();

        public void OnGet()
        {
            BestSellingProducts = _context.Products
                .Select(p => new BestSellingProductDto
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    TotalSold = _context.OrderDetails
                        .Where(od => od.ProductId == p.ProductId)
                        .Sum(od => od.Quantity) ?? 0
                })
                .OrderByDescending(p => p.TotalSold)
                .Take(5)
                .ToList();
        }
    }

    public class BestSellingProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int TotalSold { get; set; }
    }
}
