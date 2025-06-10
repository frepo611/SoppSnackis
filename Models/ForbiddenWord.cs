using System.ComponentModel.DataAnnotations;

namespace SoppSnackis.Models;

public class ForbiddenWord
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Word { get; set; } = string.Empty;

    // Add this property
    public Guid CreatedByUserId { get; set; }

    // Optional: navigation property if you want to include user info
    public Areas.Identity.Data.SoppSnackisUser? CreatedByUser { get; set; }
}
