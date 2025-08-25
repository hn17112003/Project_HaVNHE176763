using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http; // để dùng Session
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FinalProject.Models;
using System;

public class ProfileModel : PageModel
{
    private readonly ElectronicsStoreContext _context;

    public ProfileModel(ElectronicsStoreContext context)
    {
        _context = context;
    }

    public User CurrentUser { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        // Lấy user_id từ Session
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            // Nếu chưa login thì chuyển về trang login
            return RedirectToPage("/Login");
        }

        // Lấy thông tin user từ DB
        CurrentUser = await _context.Users
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (CurrentUser == null)
        {
            return NotFound("User not found");
        }

        return Page();
    }
}
