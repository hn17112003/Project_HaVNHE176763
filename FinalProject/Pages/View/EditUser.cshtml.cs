using FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FinalProject.Pages.View
{
    public class EditUserModel : PageModel
    {
        private readonly ElectronicsStoreContext _context;

        public EditUserModel(ElectronicsStoreContext context)
        {
            _context = context;
        }
        [BindProperty]
        public User User { get; set; }
        public int userEditId { get; set; }
        public List<SelectListItem> Roles { get; set; }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            userEditId = id;
            int? userId = HttpContext.Session.GetInt32("UserId");
            string role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(role) || !role.Equals("admin"))
            {
                return RedirectToPage("/View/Login");
            }
            User = await _context.Users.FindAsync(id);
            Roles = _context.Users
                            .Select(u => u.Role)
                            .Distinct()
                            .Where(role => !string.IsNullOrEmpty(role))
                            .Select(role => new SelectListItem
                            {
                                Value = role,
                                Text = role
                            }).ToList();
            if (User == null)
            {
                return NotFound();
            }
            userId = User.UserId;
            return Page();
        }
        public async Task<IActionResult> OnPost(int id)
		{
			if (!ModelState.IsValid)
            {
                Roles = _context.Users
                                .Select(u => u.Role)
                                .Distinct()
                                .Where(role => !string.IsNullOrEmpty(role))
                                .Select(role => new SelectListItem
                                {
                                    Value = role,
                                    Text = role
                                }).ToList();
                return Page(); // Trả về trang hiện tại nếu dữ liệu không hợp lệ
            }

            var userToUpdate = _context.Users.Where(x => x.UserId == id).FirstOrDefault();
            if (userToUpdate == null)
            {
                return NotFound(); // Trả về 404 nếu người dùng không tồn tại
            }

            // Cập nhật các trường cần thiết
            userToUpdate.Username = User.Username;
            userToUpdate.Email = User.Email;
            userToUpdate.Address = User.Address;
            userToUpdate.Phone = User.Phone;
            userToUpdate.Role = User.Role;
            // Cập nhật các trường khác nếu cần

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Xử lý lỗi cập nhật nếu cần
                ModelState.AddModelError(string.Empty, "Error updating the user. Please try again.");
                return Page();
            }

            return RedirectToPage("ManageAccount");
        }
    }
}
