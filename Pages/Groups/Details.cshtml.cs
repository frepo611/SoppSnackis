using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SoppSnackis.Models;
using SoppSnackis.Areas.Identity.Data;

namespace SoppSnackis.Pages.Groups;

public class DetailsModel : PageModel
{
    private readonly SoppSnackisIdentityDbContext _context;
    private readonly UserManager<SoppSnackisUser> _userManager;

    public DetailsModel(SoppSnackisIdentityDbContext context, UserManager<SoppSnackisUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public Group Group { get; set; }
    public List<GroupMember> Members { get; set; }
    public List<PrivateMessage> Messages { get; set; }
    public bool IsOwner { get; set; }
    public bool IsMember { get; set; }

    [BindProperty]
    public string NewMessage { get; set; }
    [BindProperty]
    public string InviteUserEmail { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        Group = await _context.Groups
            .Include(g => g.Members).ThenInclude(m => m.User)
            .FirstOrDefaultAsync(g => g.Id == id);
        if (Group == null) return NotFound();

        Members = Group.Members.ToList();
        IsOwner = user != null && Group.CreatedByUserId == user.Id;
        IsMember = user != null && Members.Any(m => m.UserId == user.Id);

        Messages = await _context.PrivateMessages
            .Include(m => m.Sender)
            .Where(m => m.GroupId == id && m.IsGroupMessage)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostSendMessageAsync(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();

        var isMember = await _context.GroupMembers.AnyAsync(m => m.GroupId == id && m.UserId == user.Id);
        if (!isMember) return Forbid();

        _context.PrivateMessages.Add(new PrivateMessage
        {
            SenderId = user.Id,
            GroupId = id,
            Text = NewMessage,
            CreatedAt = DateTime.UtcNow,
            IsGroupMessage = true
        });
        await _context.SaveChangesAsync();
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostInviteAsync(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        var group = await _context.Groups.FindAsync(id);
        if (group == null || group.CreatedByUserId != user.Id) return Forbid();

        var invitedUser = await _userManager.FindByEmailAsync(InviteUserEmail);
        if (invitedUser == null) return Page();

        var alreadyMember = await _context.GroupMembers.AnyAsync(m => m.GroupId == id && m.UserId == invitedUser.Id);
        if (!alreadyMember)
        {
            _context.GroupMembers.Add(new GroupMember { GroupId = id, UserId = invitedUser.Id, IsOwner = false });
            await _context.SaveChangesAsync();
        }
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostRemoveMemberAsync(int id, Guid userId)
    {
        var user = await _userManager.GetUserAsync(User);
        var group = await _context.Groups.FindAsync(id);
        if (group == null || group.CreatedByUserId != user.Id) return Forbid();

        var member = await _context.GroupMembers.FirstOrDefaultAsync(m => m.GroupId == id && m.UserId == userId);
        if (member != null)
        {
            _context.GroupMembers.Remove(member);
            await _context.SaveChangesAsync();
        }
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostLeaveGroupAsync(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();

        var member = await _context.GroupMembers.FirstOrDefaultAsync(m => m.GroupId == id && m.UserId == user.Id);
        if (member == null) return NotFound();
        if (member.IsOwner)
        {
            TempData["StatusMessage"] = "Ägaren kan inte lämna gruppen. Ta bort gruppen istället.";
            return RedirectToPage(new { id });
        }

        _context.GroupMembers.Remove(member);
        await _context.SaveChangesAsync();
        TempData["StatusMessage"] = "Du har lämnat gruppen.";
        return RedirectToPage("/Groups/Index");
    }

    public async Task<IActionResult> OnPostDeleteGroupAsync(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        var group = await _context.Groups
            .Include(g => g.Members)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (group == null || group.CreatedByUserId != user.Id)
            return Forbid();

        // Remove all members (including the owner)
        _context.GroupMembers.RemoveRange(group.Members);
        await _context.SaveChangesAsync();

        TempData["StatusMessage"] = "Gruppen har raderats.";
        return RedirectToPage("/Groups/Index");
    }
}