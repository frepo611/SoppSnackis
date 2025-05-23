using System.ComponentModel.DataAnnotations;

namespace SoppSnackis.Models;

public class Group
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = null!;

    public Guid CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User? CreatedByUser { get; set; }
    public ICollection<GroupMember> Members { get; set; } = new List<GroupMember>();
    public ICollection<PrivateMessage> Messages { get; set; } = new List<PrivateMessage>();
}
