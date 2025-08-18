using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinalProject.Models;


public class TopSellersModel : PageModel
{
    private readonly ElectronicsStoreContext _context;
    public List<TopSellerViewModel> TopSellers { get; set; }

    public TopSellersModel(ElectronicsStoreContext context)
    {
        _context = context;
    }

    public async Task OnGetAsync()
    {
        TopSellers = await _context.Orders
            .Where(o => o.StaffId.HasValue)
            .GroupBy(o => o.StaffId)
            .Select(g => new TopSellerViewModel
            {
                StaffName = g.First().Staff.Username,
                TotalSales = g.Sum(o => o.TotalAmount) ?? 0
            })
            .OrderByDescending(s => s.TotalSales)
            .Take(5)
            .ToListAsync();
    }
}
public class TopSellerViewModel
{
    public string StaffName { get; set; }
    public decimal TotalSales { get; set; }
}