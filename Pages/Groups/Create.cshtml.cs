using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using SoppSnackis.Models;
using SoppSnackis.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace SoppSnackis.Pages.Groups;

public class CreateModel : PageModel
{
    private readonly SoppSnackisIdentityDbContext _context;
    private readonly UserManager<SoppSnackisUser> _userManager;

    public CreateModel(SoppSnackisIdentityDbContext context, UserManager<SoppSnackisUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [BindProperty, Required, MaxLength(100)]
    public string Name { get; set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();

        var group = new Group { Name = Name, CreatedByUserId = user.Id, CreatedAt = DateTime.UtcNow };
        _context.Groups.Add(group);
        await _context.SaveChangesAsync();

        _context.GroupMembers.Add(new GroupMember { GroupId = group.Id, UserId = user.Id, IsOwner = true });
        await _context.SaveChangesAsync();

        return RedirectToPage("Details", new { id = group.Id });
    }
}
