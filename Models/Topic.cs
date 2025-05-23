using System.ComponentModel.DataAnnotations;
using SoppSnackis.Areas.Identity.Data;
namespace SoppSnackis.Models;

public class Topic
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = null!;

    public Guid CreatedByUserId { get; set; } //Foreign key to SoppSnackisUser
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public SoppSnackisUser? CreatedByUser { get; set; } // Navigation property
    public ICollection<Post> Posts { get; set; } = new List<Post>();
}
