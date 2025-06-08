using System.ComponentModel.DataAnnotations;

namespace SoppSnackis.Models;

public class Post
{
    public int Id { get; set; }

    public int SubjectId { get; set; }
    public Guid AuthorId { get; set; }
    public int? ParentPostId { get; set; } // For tree structure

    [Required]
    public string Text { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? ImagePath { get; set; }
    public string? Quote { get; set; }
    public int Likes { get; set; }

    public Topic? Subject { get; set; }
    public Areas.Identity.Data.SoppSnackisUser? Author { get; set; }
    public Post? ParentPost { get; set; }
    public ICollection<Post> Replies { get; set; } = new List<Post>();
    public ICollection<Report> Reports { get; set; } = new List<Report>();
}
