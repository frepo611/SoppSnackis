using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SoppSnackis.Services;
using System.Threading.Tasks;

namespace SoppSnackis.Pages.Admin;

[Authorize(Roles = "admin")]
public class ApiStatusModel : PageModel
{
    private readonly IApiService _apiService;

    public string? Status { get; set; }
    public string? ErrorMessage { get; set; }

    public ApiStatusModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task OnGetAsync()
    {
        try
        {
            Status = await _apiService.GetStatusAsync();
        }
        catch
        {
            ErrorMessage = "Kunde inte hämta API-status.";
        }
    }
}