using Microsoft.AspNetCore.Identity;
using SoppSnackis.Models;

namespace SoppSnackis.Areas.Identity.Data;

// Add profile data for application users by adding properties to the SoppSnackisUser class
public class SoppSnackisUser : IdentityUser<Guid>
{
    public string? ProfileImagePath { get; set; }
    public string? InfoText { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Post> Posts { get; set; } = new List<Post>();
    public ICollection<PrivateMessage> SentMessages { get; set; } = new List<PrivateMessage>();
    public ICollection<PrivateMessage> ReceivedMessages { get; set; } = new List<PrivateMessage>();
    public ICollection<GroupMember> GroupMemberships { get; set; } = new List<GroupMember>();
}

