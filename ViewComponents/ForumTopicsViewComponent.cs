using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoppSnackis.Areas.Identity.Data;

namespace SoppSnackis.ViewComponents
{
    public class ForumTopicsViewComponent : ViewComponent
    {
        private readonly SoppSnackisIdentityDbContext _context;

        public ForumTopicsViewComponent(SoppSnackisIdentityDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Retrieve forum topics from your database. Adjust the query as needed.
            var topics = await _context.Topics.OrderBy(s => s.Name).ToListAsync();
            return View(topics);
        }
    }
}
