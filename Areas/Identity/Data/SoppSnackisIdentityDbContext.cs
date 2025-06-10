using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SoppSnackis.Models;

namespace SoppSnackis.Areas.Identity.Data;

public class SoppSnackisIdentityDbContext : IdentityDbContext<SoppSnackisUser, IdentityRole<Guid>, Guid>
{
    public SoppSnackisIdentityDbContext(DbContextOptions<SoppSnackisIdentityDbContext> options)
        : base(options)
    {
    }
    public DbSet<Post> Posts { get; set; } = default!;
    public DbSet<Group> Groups { get; set; } = default!;
    public DbSet<GroupMember> GroupMembers { get; set; } = default!;
    public DbSet<PrivateMessage> PrivateMessages { get; set; } = default!;
    public DbSet<Report> Reports { get; set; } = default!;
    public DbSet<Topic> Topics { get; set; } = default!;
    public DbSet<PostLike> PostLikes { get; set; }
    public DbSet<ForbiddenWord> ForbiddenWords { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Group: CreatedByUser
        builder.Entity<Group>()
            .HasOne(g => g.CreatedByUser)
            .WithMany()
            .HasForeignKey(g => g.CreatedByUserId)
            .OnDelete(DeleteBehavior.Cascade);

        // GroupMember: User, Group
        builder.Entity<GroupMember>()
            .HasOne(gm => gm.User)
            .WithMany(u => u.GroupMemberships)
            .HasForeignKey(gm => gm.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<GroupMember>()
            .HasOne(gm => gm.Group)
            .WithMany(g => g.Members)
            .HasForeignKey(gm => gm.GroupId)
            .OnDelete(DeleteBehavior.Restrict);

        // Post: Author, ParentPost, Subject
        builder.Entity<Post>()
            .HasOne(p => p.Author)
            .WithMany(u => u.Posts)
            .HasForeignKey(p => p.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Post>()
            .HasOne(p => p.ParentPost)
            .WithMany(p => p.Replies)
            .HasForeignKey(p => p.ParentPostId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<Post>()
            .HasOne(p => p.Subject)
            .WithMany(t => t.Posts)
            .HasForeignKey(p => p.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PostLike>()
            .HasOne(pl => pl.Post)
            .WithMany()
            .HasForeignKey(pl => pl.PostId)
            .OnDelete(DeleteBehavior.Restrict); // Prevents cascade path error

        // PrivateMessage: Sender, Receiver, Group
        builder.Entity<PrivateMessage>()
            .HasOne(pm => pm.Sender)
            .WithMany(u => u.SentMessages)
            .HasForeignKey(pm => pm.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PrivateMessage>()
            .HasOne(pm => pm.Receiver)
            .WithMany(u => u.ReceivedMessages)
            .HasForeignKey(pm => pm.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PrivateMessage>()
            .HasOne(pm => pm.Group)
            .WithMany(g => g.Messages)
            .HasForeignKey(pm => pm.GroupId)
            .OnDelete(DeleteBehavior.NoAction);

        // Report: Post, ReportedByUser
        builder.Entity<Report>()
            .HasOne(r => r.Post)
            .WithMany(p => p.Reports)
            .HasForeignKey(r => r.PostId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Report>()
            .HasOne(r => r.ReportedByUser)
            .WithMany()
            .HasForeignKey(r => r.ReportedByUserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Topic: CreatedByUser
        builder.Entity<Topic>()
            .HasOne(t => t.CreatedByUser)
            .WithMany()
            .HasForeignKey(t => t.CreatedByUserId)
            .OnDelete(DeleteBehavior.Cascade);

    }

}

