using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions options) : base(options)
    {

    }


    public DbSet<AppUser> Users { get; set; }

    public DbSet<UserLike> Likes { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Configure the primary key for the UserLike entity
        builder.Entity<UserLike>()
            .HasKey(k => new { k.SourceUserId, k.TargetUserId });
        // Configure the relationship between UserLike entity and SourceUser
        builder.Entity<UserLike>()
            .HasOne(s => s.SourceUser) // SourceUser is the navigation property in UserLike entity
            .WithMany(l => l.LikedUsers)
            .HasForeignKey(s => s.SourceUserId)
            .OnDelete(DeleteBehavior.Cascade);
        // Configure the relationship between UserLike entity and TargetUser
        builder.Entity<UserLike>()
            .HasOne(s => s.TargetUser)
            .WithMany(l => l.LikedByUsers)
            .HasForeignKey(s => s.TargetUserId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
