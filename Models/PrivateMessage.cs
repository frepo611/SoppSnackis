using System.ComponentModel.DataAnnotations;

namespace SoppSnackis.Models;

public class PrivateMessage
{
    public int Id { get; set; }

    public Guid SenderId { get; set; }
    public Guid? ReceiverId { get; set; }
    public int? GroupId { get; set; }

    [Required]
    public string Text { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsGroupMessage { get; set; }

    public User? Sender { get; set; }
    public User? Receiver { get; set; }
    public Group? Group { get; set; }
}
