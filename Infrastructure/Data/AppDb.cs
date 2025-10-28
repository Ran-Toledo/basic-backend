using Microsoft.EntityFrameworkCore;
using BasicBackend.Domain;

namespace BasicBackend.Infrastructure.Data
{
    public class AppDb : DbContext
    {
        public AppDb(DbContextOptions<AppDb> options) : base(options) { }
        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<User>().HasIndex(u => u.Email).IsUnique();
            b.Entity<User>().Property(u => u.Name).HasMaxLength(100).IsRequired();
            b.Entity<User>().Property(u => u.Email).HasMaxLength(254).IsRequired();
            b.Entity<User>().Property(u => u.PasswordHash).IsRequired();
            b.Entity<User>().Property(u => u.PasswordSalt).IsRequired();
        }
    }
}
