using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SoppSnackis.Areas.Identity.Data;
using SoppSnackis.Models;
using System.ComponentModel.DataAnnotations;

namespace SoppSnackis.Pages.Users;

public class DetailsModel : PageModel
{
    private readonly UserManager<SoppSnackisUser> _userManager;

    private readonly SoppSnackisIdentityDbContext _context;
    public DetailsModel(UserManager<SoppSnackisUser> userManager, SoppSnackisIdentityDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public SoppSnackisUser? UserDetails { get; set; }

    [BindProperty]
    public string? InfoText { get; set; }

    [BindProperty]
    public IFormFile? ProfileImage { get; set; }

    [BindProperty]
    [StringLength(1000, ErrorMessage = "Meddelandet är för långt.")]
    public string? NewMessageText { get; set; }

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

    public async Task<IActionResult> OnPostSendMessageAsync(Guid id)
    {
        if (!User.Identity.IsAuthenticated)
            return Challenge();

        if (string.IsNullOrWhiteSpace(NewMessageText))
        {
            ModelState.AddModelError(nameof(NewMessageText), "Meddelandet får inte vara tomt.");
            await OnGetAsync(id);
            return Page();
        }

        var receiver = await _userManager.FindByIdAsync(id.ToString());
        var sender = await _userManager.GetUserAsync(User);
        if (receiver == null || sender == null)
            return NotFound();

        var message = new PrivateMessage
        {
            SenderId = sender.Id,
            ReceiverId = receiver.Id,
            Text = NewMessageText,
            CreatedAt = DateTime.UtcNow,
            IsGroupMessage = false
        };
        _context.PrivateMessages.Add(message);
        await _context.SaveChangesAsync();

        TempData["StatusMessage"] = "Meddelandet har skickats!";
        return RedirectToPage(new { id = receiver.Id });
    }
}