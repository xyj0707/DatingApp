using API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace API.Data;

public class DataContext : IdentityDbContext<AppUser, AppRole, int, IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
{
    public DataContext(DbContextOptions options) : base(options)
    {

    }


    // public DbSet<AppUser> Users { get; set; }

    public DbSet<UserLike> Likes { get; set; }
    public DbSet<Message> Messages { get; set; }

    public DbSet<Group> Groups { get; set; }

    public DbSet<Connection> Connections { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Configuring the relationship for AppUser and AppUserRole entities.
        builder.Entity<AppUser>()
           .HasMany(ur => ur.UserRoles) // A user can have multiple roles
           .WithOne(u => u.User) // Each UserRole has one associated User
           .HasForeignKey(ur => ur.UserId) // Foreign key relationship with UserId
           .IsRequired();
        // Configuring the relationship for AppRole and AppUserRole entities.
        builder.Entity<AppRole>()
            .HasMany(ur => ur.UserRoles)  // A role can be assigned to multiple users
            .WithOne(u => u.Role) // Each UserRole has one associated Role
            .HasForeignKey(ur => ur.RoleId)
            .IsRequired();
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

        builder.Entity<Message>()
            .HasOne(u => u.Recipient)
            .WithMany(m => m.MessageReceived)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Message>()
            .HasOne(u => u.Sender)
            .WithMany(m => m.MessageSent)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
