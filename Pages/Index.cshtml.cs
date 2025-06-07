using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SoppSnackis.Areas.Identity.Data;
using SoppSnackis.Models;

public class IndexModel : PageModel
{
    private readonly SoppSnackisIdentityDbContext _context;

    public List<Topic> Topics { get; set; } = new();

    public IndexModel(SoppSnackisIdentityDbContext context)
    {
        _context = context;
    }

    public async Task OnGetAsync()
    {
        Topics = await _context.Topics.OrderBy(t => t.Name).ToListAsync();
    }
}
