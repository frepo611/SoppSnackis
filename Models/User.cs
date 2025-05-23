using System.ComponentModel.DataAnnotations;

namespace SoppSnackis.Models;

public class User
{
    public Guid Id { get; set; }

    [Required, MaxLength(50)]
    public string UserName { get; set; } = null!;

    [Required, MaxLength(100)]
    public string Email { get; set; } = null!;

    [Required]
    public string PasswordHash { get; set; } = null!;

    public string? ProfileImagePath { get; set; }
    public string? InfoText { get; set; }
    [Required]
    public string Role { get; set; } = "User";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Post> Posts { get; set; } = new List<Post>();
    public ICollection<PrivateMessage> SentMessages { get; set; } = new List<PrivateMessage>();
    public ICollection<PrivateMessage> ReceivedMessages { get; set; } = new List<PrivateMessage>();
    public ICollection<GroupMember> GroupMemberships { get; set; } = new List<GroupMember>();
}
