using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SoppSnackis.Models;
using SoppSnackis.DTOs;
using System.ComponentModel.DataAnnotations;

namespace SoppSnackis.Pages.Admin;

[Authorize(Roles = "admin")]
public class TopicsModel : PageModel
{
    [BindProperty]
    [Required(ErrorMessage = "�mnesnamn �r obligatoriskt.")]
    [Display(Name = "�mnesnamn")]
    public TopicDTO NewTopic { get; set; } = new();

    public List<TopicDTO>? Topics { get; set; }
    public string? ErrorMessage { get; set; }

    private readonly Services.IApiService _apiService;

    public TopicsModel(Services.IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task OnGetAsync()
    {
        try
        {
            Topics = await _apiService.GetTopicsAsync();
        }
        catch
        {
            ErrorMessage = "Kunde inte ladda �mnen.";
        }
    }

    public async Task<IActionResult> OnPostCreateAsync()
    {
        // Remove CreatedByUserName from ModelState before validation
        ModelState.Remove("NewTopic.CreatedByUserName");

        if (!ModelState.IsValid)
        {
            await OnGetAsync();
            return Page();
        }

        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId != null && Guid.TryParse(userId, out var guid))
            {
                NewTopic.CreatedByUserId = guid;
                NewTopic.CreatedAt = DateTime.Now;
            }
            else
            {
                ErrorMessage = "Kunde inte identifiera anv�ndare.";
                await OnGetAsync();
                return Page();
            }
            await _apiService.CreateTopicAsync(NewTopic);
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
            await _apiService.DeleteTopicAsync(id);
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

