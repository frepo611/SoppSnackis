using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SoppSnackis.Areas.Identity.Data;

namespace SoppSnackis.Pages.Admin
{
    [Authorize(Roles = "admin")]
    public class UsersModel : PageModel
    {
        private readonly UserManager<SoppSnackisUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public class UserViewModel
        {
            public Guid Id { get; set; }
            public string Email { get; set; } = string.Empty;
            public List<string> Roles { get; set; } = new();
            public DateTime CreatedAt { get; set; }
        }

        public List<UserViewModel> Users { get; set; } = new();

        public UsersModel(UserManager<SoppSnackisUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task OnGetAsync()
        {
            var users = _userManager.Users.ToList();
            Users = new List<UserViewModel>();
            foreach (var user in users)
            {
                var roles = (await _userManager.GetRolesAsync(user)).ToList();
                Users.Add(new UserViewModel
                {
                    Id = user.Id,
                    Email = user.Email ?? "",
                    Roles = roles,
                    CreatedAt = user.CreatedAt
                });
            }
        }

        public async Task<IActionResult> OnPostPromoteAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user != null && !(await _userManager.IsInRoleAsync(user, "admin")))
            {
                await _userManager.AddToRoleAsync(user, "admin");
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDemoteAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user != null && await _userManager.IsInRoleAsync(user, "admin"))
            {
                await _userManager.RemoveFromRoleAsync(user, "admin");
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
            return RedirectToPage();
        }
    }
}
