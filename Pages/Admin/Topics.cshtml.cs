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
    [Required(ErrorMessage = "Ämnesnamn är obligatoriskt.")]
    [Display(Name = "Ämnesnamn")]
    public string NewTopicName { get; set; } = string.Empty;

    public List<Topic>? Topics { get; set; }
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        try
        {
            // TODO: Anropa API för att hämta ämnen
            Topics = new List<Topic>(); // Ersätt med API-anrop
        }
        catch
        {
            ErrorMessage = "Kunde inte ladda ämnen.";
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
            // TODO: Anropa API för att skapa ämne
            // Exempel: await _apiService.CreateTopicAsync(NewTopicName);
            return RedirectToPage();
        }
        catch
        {
            ErrorMessage = "Kunde inte skapa ämne.";
            await OnGetAsync();
            return Page();
        }
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        try
        {
            // TODO: Anropa API för att ta bort ämne
            // Exempel: await _apiService.DeleteTopicAsync(id);
            return RedirectToPage();
        }
        catch
        {
            ErrorMessage = "Kunde inte ta bort ämne.";
            await OnGetAsync();
            return Page();
        }
    }
}

