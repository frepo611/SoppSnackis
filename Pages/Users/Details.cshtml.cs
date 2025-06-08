using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using SoppSnackis.Areas.Identity.Data;
using System.Threading.Tasks;
using System;

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

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            UserDetails = await _userManager.FindByIdAsync(id.ToString());
            if (UserDetails == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
