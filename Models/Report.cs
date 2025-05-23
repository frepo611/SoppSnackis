using System.ComponentModel.DataAnnotations;

namespace SoppSnackis.Models;

public class Report
{
    public int Id { get; set; }

    public int PostId { get; set; }
    public Guid ReportedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public string Status { get; set; } = "Open"; // Open, Reviewed, Closed

    public string? Comment { get; set; }

    public Post? Post { get; set; }
    public User? ReportedByUser { get; set; }
}
