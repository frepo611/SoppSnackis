using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SoppSnackis.Areas.Identity.Data;
using SoppSnackis.Models;
using System.ComponentModel.DataAnnotations;

namespace SoppSnackis.Pages.Posts;

public class DetailsModel : PageModel
{
    private readonly SoppSnackisIdentityDbContext _context;
    private readonly UserManager<SoppSnackisUser> _userManager;

    public DetailsModel(SoppSnackisIdentityDbContext context, UserManager<SoppSnackisUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    public Post? Post { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Texten får inte vara tom.")]
    public string? EditText { get; set; }

    public bool CanEdit { get; set; }
    public async Task<IActionResult> OnGetAsync(int id)
    {
        Post = await _context.Posts
            .Include(p => p.Author)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (Post == null)
            return NotFound();

        var user = await _userManager.GetUserAsync(User);
        CanEdit = user != null && (User.IsInRole("admin") || Post.AuthorId == user.Id);

        if (CanEdit)
            EditText = Post.Text;

        return Page();
    }

    public async Task<IActionResult> OnPostEditAsync(int id)
    {
        Post = await _context.Posts.FindAsync(id);
        if (Post == null)
            return NotFound();

        var user = await _userManager.GetUserAsync(User);
        var isAdmin = User.IsInRole("admin");
        if (user == null || (!isAdmin && Post.AuthorId != user.Id))
            return Forbid();

        if (!ModelState.IsValid)
        {
            CanEdit = isAdmin || (user != null && Post.AuthorId == user.Id);
            return Page();
        }

        Post.Text = EditText!;
        await _context.SaveChangesAsync();

        TempData["StatusMessage"] = "Inlägget har uppdaterats.";
        return RedirectToPage(new { id });
    }
}