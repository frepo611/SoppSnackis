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

    [BindProperty]
    public IFormFile? NewImage { get; set; }

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

    public async Task<IActionResult> OnPostChangeImageAsync(int id)
    {
        var post = await _context.Posts.FindAsync(id);
        if (post == null) return NotFound();

        if (NewImage != null && NewImage.Length > 0)
        {
            var directory = Path.Combine("wwwroot/images/posts");
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var fileName = $"{NanoidDotNet.Nanoid.Generate()}{Path.GetExtension(NewImage.FileName)}";
            var filePath = Path.Combine(directory, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await NewImage.CopyToAsync(stream);
            }

            post.ImagePath = $"/images/posts/{fileName}";
            await _context.SaveChangesAsync();
        }

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostDeleteImageAsync(int id)
    {
        var post = await _context.Posts.FindAsync(id);
        if (post == null) return NotFound();

        if (!string.IsNullOrEmpty(post.ImagePath))
        { 
            post.ImagePath = null;
            await _context.SaveChangesAsync();
        }

        return RedirectToPage(new { id });
    }
}