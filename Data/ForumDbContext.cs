using Microsoft.EntityFrameworkCore;
using SoppSnackis.Models;

namespace SoppSnackis.Data;

public class ForumDbContext : DbContext
{
    public ForumDbContext(DbContextOptions<ForumDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Report> Reports => Set<Report>();
    public DbSet<PrivateMessage> PrivateMessages => Set<PrivateMessage>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<GroupMember> GroupMembers => Set<GroupMember>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // User - Post (one-to-many)
        modelBuilder.Entity<Post>()
            .HasOne(p => p.Author)
            .WithMany(u => u.Posts)
            .HasForeignKey(p => p.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Subject - Post (one-to-many)
        modelBuilder.Entity<Post>()
            .HasOne(p => p.Subject)
            .WithMany(s => s.Posts)
            .HasForeignKey(p => p.SubjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // Post - Replies (self-referencing, tree structure)
        modelBuilder.Entity<Post>()
            .HasOne(p => p.ParentPost)
            .WithMany(p => p.Replies)
            .HasForeignKey(p => p.ParentPostId)
            .OnDelete(DeleteBehavior.Restrict);

        // Post - Report (one-to-many)
        modelBuilder.Entity<Report>()
            .HasOne(r => r.Post)
            .WithMany(p => p.Reports)
            .HasForeignKey(r => r.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        // User - Report (one-to-many)
        modelBuilder.Entity<Report>()
            .HasOne(r => r.ReportedByUser)
            .WithMany()
            .HasForeignKey(r => r.ReportedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // User - PrivateMessage (Sender)
        modelBuilder.Entity<PrivateMessage>()
            .HasOne(m => m.Sender)
            .WithMany(u => u.SentMessages)
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        // User - PrivateMessage (Receiver)
        modelBuilder.Entity<PrivateMessage>()
            .HasOne(m => m.Receiver)
            .WithMany(u => u.ReceivedMessages)
            .HasForeignKey(m => m.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        // Group - PrivateMessage (one-to-many)
        modelBuilder.Entity<PrivateMessage>()
            .HasOne(m => m.Group)
            .WithMany(g => g.Messages)
            .HasForeignKey(m => m.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        // Group - GroupMember (one-to-many)
        modelBuilder.Entity<GroupMember>()
            .HasOne(gm => gm.Group)
            .WithMany(g => g.Members)
            .HasForeignKey(gm => gm.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        // User - GroupMember (one-to-many)
        modelBuilder.Entity<GroupMember>()
            .HasOne(gm => gm.User)
            .WithMany(u => u.GroupMemberships)
            .HasForeignKey(gm => gm.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Subject - CreatedByUser (optional, one-to-many)
        modelBuilder.Entity<Subject>()
            .HasOne(s => s.CreatedByUser)
            .WithMany()
            .HasForeignKey(s => s.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Group - CreatedByUser (optional, one-to-many)
        modelBuilder.Entity<Group>()
            .HasOne(g => g.CreatedByUser)
            .WithMany()
            .HasForeignKey(g => g.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        base.OnModelCreating(modelBuilder);
    }
}
