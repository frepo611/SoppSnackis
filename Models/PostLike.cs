using System;
using System.ComponentModel.DataAnnotations;

namespace SoppSnackis.Models;

public class PostLike
{
    public int Id { get; set; }

    [Required]
    public int PostId { get; set; }

    [Required]
    public Guid UserId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property to the liked post
    public Post? Post { get; set; }

    // Navigation property to the user who liked the post
    public Areas.Identity.Data.SoppSnackisUser? User { get; set; }
}
