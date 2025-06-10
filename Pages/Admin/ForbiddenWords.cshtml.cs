using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SoppSnackis.Models;
using SoppSnackis.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SoppSnackis.Pages.ForbiddenWords
{
    [Authorize(Roles = "admin")]
    public class IndexModel : PageModel
    {
        private readonly IApiService _apiService;

        public IndexModel(IApiService apiService)
        {
            _apiService = apiService;
        }

        public List<ForbiddenWord>? ForbiddenWords { get; set; }
        [BindProperty]
        public string? EditWord { get; set; }
        [BindProperty]
        public string? NewWord { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            try
            {
                ForbiddenWords = await _apiService.GetForbiddenWordsAsync();
            }
            catch
            {
                ErrorMessage = "Kunde inte ladda otillåtna ord.";
            }
        }

        public async Task<IActionResult> OnPostAddAsync()
        {
            if (string.IsNullOrWhiteSpace(NewWord))
            {
                ErrorMessage = "Nytt ord får inte vara tomt.";
                await OnGetAsync();
                return Page();
            }

            try
            {
                var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
                await _apiService.CreateForbiddenWordAsync(NewWord, userId);
                return RedirectToPage();
            }
            catch
            {
                ErrorMessage = "Kunde inte lägga till otillåtet ord.";
                await OnGetAsync();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            try
            {
                await _apiService.DeleteForbiddenWordAsync(id);
                return RedirectToPage();
            }
            catch
            {
                ErrorMessage = "Kunde inte ta bort otillåtet ord.";
                await OnGetAsync();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostEditAsync(int id)
        {
            if (string.IsNullOrWhiteSpace(EditWord))
            {
                ErrorMessage = "Ordet får inte vara tomt.";
                await OnGetAsync();
                return Page();
            }

            try
            {
                await _apiService.UpdateForbiddenWordAsync(id, EditWord);
                return RedirectToPage();
            }
            catch
            {
                ErrorMessage = "Kunde inte uppdatera otillåtet ord.";
                await OnGetAsync();
                return Page();
            }
        }
    }
}
