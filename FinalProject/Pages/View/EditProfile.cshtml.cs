using FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;

public class EditProfileModel : PageModel
{
    private readonly ElectronicsStoreContext _context;

    public EditProfileModel(ElectronicsStoreContext context)
    {
        _context = context;
    }

    [BindProperty]
    public User CurrentUser { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/View/Login");

        CurrentUser = await _context.Users.FindAsync(userId);
        if (CurrentUser == null) return NotFound();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var userId = HttpContext.Session.GetInt32("UserId");
        var userDb = await _context.Users.FindAsync(userId);
        if (userDb == null) return NotFound();

        // C?p nh?t thông tin (tr? password, role)
        userDb.Username = CurrentUser.Username;
        userDb.Email = CurrentUser.Email;
        userDb.Address = CurrentUser.Address;
        userDb.Phone = CurrentUser.Phone;
        userDb.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();

        return RedirectToPage("/View/Profile");
    }
}
