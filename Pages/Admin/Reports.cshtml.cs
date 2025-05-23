using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using SoppSnackis.Areas.Identity.Data;
using SoppSnackis.Models;

namespace SoppSnackis.Pages.Admin;

[Authorize(Roles = "admin")]
public class ReportsModel : PageModel
{
    private readonly SoppSnackisIdentityDbContext _context;
    private readonly UserManager<SoppSnackisUser> _userManager;

    public class ReportViewModel
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string ReportedByUserEmail { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Comment { get; set; }
    }

    public List<ReportViewModel> Reports { get; set; } = new();

    public ReportsModel(SoppSnackisIdentityDbContext context, UserManager<SoppSnackisUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task OnGetAsync()
    {
        Reports = await _context.Reports
            .Include(r => r.ReportedByUser)
            .Select(r => new ReportViewModel
            {
                Id = r.Id,
                PostId = r.PostId,
                ReportedByUserEmail = r.ReportedByUser.Email ?? "",
                CreatedAt = r.CreatedAt,
                Status = r.Status,
                Comment = r.Comment
            })
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostResolveAsync(int id)
    {
        var report = await _context.Reports.FindAsync(id);
        if (report != null)
        {
            report.Status = "Avslutad";
            await _context.SaveChangesAsync();
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeletePostAsync(int postId)
    {
        var post = await _context.Posts.FindAsync(postId);
        if (post != null)
        {
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
        }
        return RedirectToPage();
    }
}
