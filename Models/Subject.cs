using System.ComponentModel.DataAnnotations;

namespace SoppSnackis.Models;

public class Subject
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = null!;

    public Guid CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User? CreatedByUser { get; set; }
    public ICollection<Post> Posts { get; set; } = new List<Post>();
}
