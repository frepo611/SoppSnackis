using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SoppSnackis.Models;
using SoppSnackis.Areas.Identity.Data;

namespace SoppSnackis.Pages.Groups;

public class IndexModel : PageModel
{
    private readonly SoppSnackisIdentityDbContext _context;
    private readonly UserManager<SoppSnackisUser> _userManager;

    public IndexModel(SoppSnackisIdentityDbContext context, UserManager<SoppSnackisUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public List<GroupMember> MyGroupMemberships { get; set; } = new();

    public async Task OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return;

        MyGroupMemberships = await _context.GroupMembers
            .Include(m => m.Group)
            .Where(m => m.UserId == user.Id)
            .ToListAsync();
    }
}
