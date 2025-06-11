using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SoppSnackis.Areas.Identity.Data;
using SoppSnackis.Models;
using System.ComponentModel.DataAnnotations;

namespace SoppSnackis.Pages.Messages;

public class IndexModel : PageModel
{
    private readonly UserManager<SoppSnackisUser> _userManager;
    private readonly SoppSnackisIdentityDbContext _context;

    public IndexModel(UserManager<SoppSnackisUser> userManager, SoppSnackisIdentityDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public List<PrivateMessage> ReceivedMessages { get; set; } = new();
    public List<PrivateMessage> SentMessages { get; set; } = new();
    [BindProperty]
    [Display(Name = "Svar")]
    [StringLength(1000, ErrorMessage = "Svaret är för långt.")]
    public string? ReplyText { get; set; }

    public async Task<IActionResult> OnPostReplyAsync(Guid receiverId)
    {
        if (string.IsNullOrWhiteSpace(ReplyText))
        {
            ModelState.AddModelError("ReplyText", "Svaret får inte vara tomt.");
            await OnGetAsync();
            return Page();
        }

        var sender = await _userManager.GetUserAsync(User);
        if (sender == null)
            return Challenge();

        var receiver = await _userManager.FindByIdAsync(receiverId.ToString());
        if (receiver == null)
            return NotFound();

        var message = new PrivateMessage
        {
            SenderId = sender.Id,
            ReceiverId = receiver.Id,
            Text = ReplyText,
            CreatedAt = DateTime.UtcNow,
            IsGroupMessage = false
        };

        _context.PrivateMessages.Add(message);
        await _context.SaveChangesAsync();

        ReplyText = string.Empty;

        return RedirectToPage();
    }
    public async Task OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return;

        ReceivedMessages = await _context.PrivateMessages
            .Include(m => m.Sender)
            .Where(m => m.ReceiverId == user.Id && !m.IsGroupMessage)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();

        SentMessages = await _context.PrivateMessages
            .Include(m => m.Receiver)
            .Where(m => m.SenderId == user.Id && !m.IsGroupMessage)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
    }
}
