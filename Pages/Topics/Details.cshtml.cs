using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NanoidDotNet;
using SoppSnackis.Areas.Identity.Data;
using SoppSnackis.Models;
using SoppSnackis.Utilities;
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
    private readonly WordFilter _wordFilter;


    public Post NewReply { get; set; } = new();
    public Topic? Topic { get; set; }

    public int TopicId { get; set; }
    public List<Post> Posts { get; set; } = new();
    public List<PostWithLevel> PostsWithLevel { get; set; } = new();

    [BindProperty]
    [Required(ErrorMessage = "Inl�ggstext �r obligatorisk.")]
    [Display(Name = "Inl�gg")]
    public string NewPostText { get; set; }
    [BindProperty]
    [Required(ErrorMessage = " Svarstext �r obligatorisk.")]
    [Display(Name = "Svar")]
    public string NewReplyText { get; set; }

    [BindProperty]
    public IFormFile NewPostImage { get; set; }

    public DetailsModel(SoppSnackisIdentityDbContext context, UserManager<SoppSnackisUser> userManager, WordFilter wordFilter)
    {
        _context = context;
        _userManager = userManager;
        _wordFilter = wordFilter;
    }
    public async Task<IActionResult> OnGetAsync(int id)
    {
        if (id <= 0)
        {
            return NotFound();
        }

        Topic = await _context.Topics
            .Include(t => t.Posts)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (Topic == null)
        {
            return NotFound();
        }

        TopicId = Topic.Id;

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
    public async Task<IActionResult> OnPostLikeAsync(int postId, int topicId)
    {
        var post = await _context.Posts.FindAsync(postId);
        if (post == null)
            return NotFound();

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Forbid();

        // Check if the user has already liked this post
        bool alreadyLiked = await _context.PostLikes
            .AnyAsync(pl => pl.PostId == postId && pl.UserId == user.Id);

        if (alreadyLiked)
        {
            TempData["StatusMessage"] = "Du har redan gillat detta inl�gg.";
            return RedirectToPage(new { id = topicId });
        }

        // Add like record
        var postLike = new PostLike
        {
            PostId = postId,
            UserId = user.Id
        };
        _context.PostLikes.Add(postLike);

        post.Likes += 1;
        await _context.SaveChangesAsync();

        return RedirectToPage(new { id = topicId });
    }
    public async Task<IActionResult> OnPostReplyAsync(int parentId)
    {
        ModelState.Remove(nameof(NewPostText)); // Ignore new post validation
        ModelState.Remove(nameof(NewPostImage)); // Ignore new image validation
        if (!ModelState.IsValid)
        {
            await OnGetAsync(parentId); // or topic id
            return Page();
        }

        var parentPost = await _context.Posts.FindAsync(parentId);
        if (parentPost == null)
            return NotFound();

        var user = await _userManager.GetUserAsync(User);

        var filterResult = await _wordFilter.FilterForbiddenWordsAsync(NewReplyText);
        var filteredText = filterResult.filteredText;
        var hadForbidden = filterResult.hadForbidden;

        var reply = new Post
        {
            Text = filteredText,
            AuthorId = user.Id,
            CreatedAt = DateTime.UtcNow,
            ParentPostId = parentId,
            SubjectId = parentPost.SubjectId
        };

        _context.Posts.Add(reply);
        await _context.SaveChangesAsync();

        if (hadForbidden)
            TempData["StatusMessage"] = "Vissa ord i ditt inl�gg var otill�tna och har ersatts med ***.";

        return RedirectToPage(new { id = parentPost.SubjectId });
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        ModelState.Remove(nameof(NewReplyText)); // Ignore reply validation
        ModelState.Remove(nameof(NewPostImage)); // Ignore image validation
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

        string? imagePath = null;
        if (NewPostImage != null && NewPostImage.Length > 0)
        {
            var directory = Path.Combine("wwwroot/images/posts");
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var fileName = $"{Nanoid.Generate()}{Path.GetExtension(NewPostImage.FileName)}";
            var filePath = Path.Combine(directory, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await NewPostImage.CopyToAsync(stream);
            }

            imagePath = $"/images/posts/{fileName}";
        }

        var filterResult = await _wordFilter.FilterForbiddenWordsAsync(NewPostText);
        var filteredText = filterResult.filteredText;
        var hadForbidden = filterResult.hadForbidden;


        var post = new Post
        {
            SubjectId = id,
            AuthorId = user.Id,
            Text = filteredText,
            CreatedAt = DateTime.UtcNow,
            ImagePath = imagePath
        };

        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

        if (hadForbidden)
            TempData["StatusMessage"] = "Vissa ord i ditt inl�gg var otill�tna och har ersatts med ***.";

        // Clear the form after submission
        NewPostText = string.Empty;

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostReportAsync(int postId, int topicId, string? comment)
    { 
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();

        // Prevent duplicate reports by the same user for the same post (optional)
        var alreadyReported = await _context.Reports
            .AnyAsync(r => r.PostId == postId && r.ReportedByUserId == user.Id && r.Status == "Open");
        if (alreadyReported)
        {
            TempData["StatusMessage"] = "Du har redan rapporterat detta inl�gg.";
            return RedirectToPage(new { id = topicId });
        }

        var report = new Report
        {
            PostId = postId,
            ReportedByUserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            Status = "Open",
            Comment = comment
        };
        _context.Reports.Add(report);
        await _context.SaveChangesAsync();

        TempData["StatusMessage"] = "Inl�gget har rapporterats.";
        return RedirectToPage(new { id = topicId });
    }
}