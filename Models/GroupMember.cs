namespace SoppSnackis.Models;

public class GroupMember
{
    public int Id { get; set; }

    public int GroupId { get; set; }
    public Guid UserId { get; set; }
    public bool IsOwner { get; set; }

    public Group? Group { get; set; }
    public Areas.Identity.Data.SoppSnackisUser? User { get; set; }
}
