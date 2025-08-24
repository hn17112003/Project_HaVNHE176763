using FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.ComponentModel;

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

        // upload file excel
        public async Task<IActionResult> OnPostUploadExcelAsync(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                ModelState.AddModelError("", "Vui lòng chọn file Excel.");
                return Page();
            }

            using (var stream = new MemoryStream())
            {
                await excelFile.CopyToAsync(stream);
                stream.Position = 0;

                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    if (worksheet == null)
                    {
                        ModelState.AddModelError("", "File Excel không có dữ liệu.");
                        return Page();
                    }

                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++) // bỏ qua header
                    {
                        if (worksheet.Cells[row, 1].Value == null) continue;

                        var product = new Product
                        {
                            ProductName = worksheet.Cells[row, 1].Text,
                            Price = decimal.TryParse(worksheet.Cells[row, 2].Text, out var price) ? price : 0,
                            StockQuantity = int.TryParse(worksheet.Cells[row, 3].Text, out var qty) ? qty : 0,
                            CategoryId = int.TryParse(worksheet.Cells[row, 4].Text, out var catId) ? catId : 0,
                            Description = worksheet.Cells[row, 5].Text,
                            ImageUrl = worksheet.Cells[row, 6].Text,
                            UserId = HttpContext.Session.GetInt32("UserId")
                        };

                        await _context.Products.AddAsync(product);
                    }

                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToPage("ManageProduct");
        }
    }
}
