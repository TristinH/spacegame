using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SpaceGame.Data.Models;

namespace SpaceGame.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Group> Groups { get; set; }
        public DbSet<Player> Players { get; set; }

        // TODO: Delete this maybe
        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    builder.Entity<Player>().HasIndex(p => p.User).IsUnique();
        //    base.OnModelCreating(builder);
        //}
    }
}
