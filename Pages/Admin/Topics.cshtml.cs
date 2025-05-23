using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SoppSnackis.Models;
using System.ComponentModel.DataAnnotations;

namespace SoppSnackis.Pages.Admin;

[Authorize(Roles = "admin")]
public class TopicsModel : PageModel
{
    [BindProperty]
    [Required(ErrorMessage = "�mnesnamn �r obligatoriskt.")]
    [Display(Name = "�mnesnamn")]
    public string NewTopicName { get; set; } = string.Empty;

    public List<Topic>? Topics { get; set; }
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        try
        {
            // TODO: Anropa API f�r att h�mta �mnen
            Topics = new List<Topic>(); // Ers�tt med API-anrop
        }
        catch
        {
            ErrorMessage = "Kunde inte ladda �mnen.";
        }
    }

    public async Task<IActionResult> OnPostCreateAsync()
    {
        if (!ModelState.IsValid)
        {
            await OnGetAsync();
            return Page();
        }

        try
        {
            // TODO: Anropa API f�r att skapa �mne
            // Exempel: await _apiService.CreateTopicAsync(NewTopicName);
            return RedirectToPage();
        }
        catch
        {
            ErrorMessage = "Kunde inte skapa �mne.";
            await OnGetAsync();
            return Page();
        }
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        try
        {
            // TODO: Anropa API f�r att ta bort �mne
            // Exempel: await _apiService.DeleteTopicAsync(id);
            return RedirectToPage();
        }
        catch
        {
            ErrorMessage = "Kunde inte ta bort �mne.";
            await OnGetAsync();
            return Page();
        }
    }
}

