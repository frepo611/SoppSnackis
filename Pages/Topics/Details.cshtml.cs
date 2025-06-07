using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SoppSnackis.Models;
using SoppSnackis.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

[Authorize(Roles = "user")]
public class DetailsModel : PageModel
{
    private readonly SoppSnackisIdentityDbContext _context;
    private readonly UserManager<SoppSnackisUser> _userManager;

    public DetailsModel(SoppSnackisIdentityDbContext context, UserManager<SoppSnackisUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    [BindProperty]
    public Post NewReply { get; set; } = new();
    public Topic? Topic { get; set; }
    public List<Post> Posts { get; set; } = new();

    [BindProperty]
    [Required(ErrorMessage = "Inläggstext är obligatorisk.")]
    [Display(Name = "Inlägg")]
    public string NewPostText { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Topic = await _context.Topics
            .Include(t => t.Posts)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (Topic == null)
        {
            return NotFound();
        }

        Posts = await _context.Posts
            .Where(p => p.SubjectId == id)
            .Include(p => p.Author)
            .Include(p => p.Replies)
                .ThenInclude(r => r.Author)
            .OrderBy(p => p.CreatedAt)
            .ToListAsync();

        return Page();
    }
    public async Task<IActionResult> OnPostReplyAsync(int parentId)
    {
        var parentPost = await _context.Posts.FindAsync(parentId);
        if (parentPost == null)
            return NotFound();

        var user = await _userManager.GetUserAsync(User);

        var reply = new Post
        {
            Text = NewReply.Text,
            AuthorId = user.Id,
            CreatedAt = DateTime.UtcNow,
            ParentPostId = parentId,
            SubjectId = parentPost.SubjectId
        };

        _context.Posts.Add(reply);
        await _context.SaveChangesAsync();

        return RedirectToPage(new { id = parentPost.SubjectId });
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!ModelState.IsValid)
        {
            await OnGetAsync(id);
            return Page();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Forbid();
        }

        var post = new Post
        {
            SubjectId = id,
            AuthorId = user.Id,
            Text = NewPostText,
            CreatedAt = DateTime.Now
        };

        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

        // Clear the form after submission
        NewPostText = string.Empty;

        return RedirectToPage(new { id });
    }
}