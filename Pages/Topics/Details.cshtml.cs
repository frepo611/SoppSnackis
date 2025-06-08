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
    public class PostWithLevel
    {
        public Post? Post { get; set; }
        public int Level { get; set; }
    }
    private readonly SoppSnackisIdentityDbContext _context;
    private readonly UserManager<SoppSnackisUser> _userManager;

    public DetailsModel(SoppSnackisIdentityDbContext context, UserManager<SoppSnackisUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    public Post NewReply { get; set; } = new();
    public Topic? Topic { get; set; }
    public List<Post> Posts { get; set; } = new();
    public List<PostWithLevel> PostsWithLevel { get; set; } = new();

    [BindProperty]
    [Required(ErrorMessage = "Inläggstext är obligatorisk.")]
    [Display(Name = "Inlägg")]
    public string NewPostText { get; set; }
    [BindProperty]
    [Required(ErrorMessage = " Svarstext är obligatorisk.")]
    [Display(Name = "Svar")]
    public string NewReplyText { get; set; }

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

        PostsWithLevel = FlattenPostsWithLevel(Posts.Where(p => p.ParentPostId == null), Posts, 0).ToList();

        return Page();
    }

    private IEnumerable<PostWithLevel> FlattenPostsWithLevel(IEnumerable<Post> posts, List<Post> allPosts, int level)
    {
        // For level 0, order by newest first; for replies, order by oldest first
        var orderedPosts = (level == 0)
            ? posts.OrderByDescending(p => p.CreatedAt)
            : posts.OrderBy(p => p.CreatedAt);

        foreach (var post in orderedPosts)
        {
            yield return new PostWithLevel { Post = post, Level = level };
            var replies = allPosts.Where(p => p.ParentPostId == post.Id);
            foreach (var reply in FlattenPostsWithLevel(replies, allPosts, level + 1))
            {
                yield return reply;
            }
        }
    }

    public async Task<IActionResult> OnPostReplyAsync(int parentId)
    {
        ModelState.Remove(nameof(NewPostText)); // Ignore new post validation
        if (!ModelState.IsValid)
        {
            await OnGetAsync(parentId); // or topic id
            return Page();
        }

        var parentPost = await _context.Posts.FindAsync(parentId);
        if (parentPost == null)
            return NotFound();

        var user = await _userManager.GetUserAsync(User);

        var reply = new Post
        {
            Text = NewReplyText,
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
        ModelState.Remove(nameof(NewReplyText)); // Ignore reply validation
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