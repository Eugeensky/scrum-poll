using Microsoft.EntityFrameworkCore;
using DAL.Helpers;

namespace DAL.Entities;

internal class AppDbContext : DbContext
{
    internal DbSet<User> Users { get; set; } = null!;
    internal DbSet<Role> Roles { get; set; } = null!;
    internal DbSet<Poll> Polls { get; set; } = null!;
    internal DbSet<PollOption> PollOptions { get; set; } = null!;
    internal DbSet<PollSession> PollSessions { get; set; } = null!;
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PollSession>().HasOne(pa => pa.PollOption).WithMany(po => po.Sessions).OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<PollSession>().HasOne(pa => pa.User).WithMany(u => u.Sessions).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<PollSession>().HasOne(pa => pa.Poll).WithMany(p => p.Sessions).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<PollSession>().HasAlternateKey(pa => new { pa.PollId, pa.UserId });

        var hashedPassword = Hasher.HashString("Qazaq123");
        Role masterRole = new() { Id = Guid.NewGuid(), Name = "Master" };
        Role userRole = new() { Id = Guid.NewGuid(), Name = "User" };
        User master = new() { Id = Guid.NewGuid(), Login = "master", Password = hashedPassword, RoleId = masterRole.Id };
        User user1 = new() { Id = Guid.NewGuid(), Login = "user1", Password = hashedPassword, RoleId = userRole.Id };
        User user2 = new() { Id = Guid.NewGuid(), Login = "user2", Password = hashedPassword, RoleId = userRole.Id };
        User user3 = new() { Id = Guid.NewGuid(), Login = "user3", Password = hashedPassword, RoleId = userRole.Id };

        modelBuilder.Entity<Role>().HasData(masterRole, userRole);
        modelBuilder.Entity<User>().HasData(master, user1, user2, user3);
    }
}
