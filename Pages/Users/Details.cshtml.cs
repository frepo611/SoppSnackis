using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using SoppSnackis.Areas.Identity.Data;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace SoppSnackis.Pages.User
{
    public class DetailsModel : PageModel
    {
        private readonly UserManager<SoppSnackisUser> _userManager;

        public DetailsModel(UserManager<SoppSnackisUser> userManager)
        {
            _userManager = userManager;
        }

        public SoppSnackisUser? UserDetails { get; set; }

        [BindProperty]
        public string? InfoText { get; set; }

        [BindProperty]
        public IFormFile? ProfileImage { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            UserDetails = await _userManager.FindByIdAsync(id.ToString());
            if (UserDetails == null)
                return NotFound();

            InfoText = UserDetails.InfoText;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return NotFound();

            // Update info text
            user.InfoText = InfoText;

            // Handle image upload
            if (ProfileImage != null && ProfileImage.Length > 0)
            {
                // Ensure the directory exists
                var directory = Path.Combine("wwwroot/images/profiles");
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Save the file (example: wwwroot/images/profiles/{userId}.jpg)
                var fileName = $"{user.Id}{Path.GetExtension(ProfileImage.FileName)}";
                var filePath = Path.Combine(directory, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ProfileImage.CopyToAsync(stream);
                }

                // Save the relative path to the user
                user.ProfileImagePath = $"/images/profiles/{fileName}";
            }

            await _userManager.UpdateAsync(user);

            // Optionally reload the user details
            UserDetails = user;
            InfoText = user.InfoText;

            return RedirectToPage(new { id = user.Id });
        }
    }
}
