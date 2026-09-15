using Microsoft.EntityFrameworkCore;
using UserRegistration.Model;

namespace UserRegistration.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasIndex(x => x.Login)
                .IsUnique();

            entity.Property(x => x.Login)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(x => x.PasswordHash)
                .IsRequired();

            entity.Property(x => x.PasswordSalt)
                .IsRequired();

            entity.Property(x => x.Name)
                .HasMaxLength(100);

            entity.Property(x => x.Email)
                .HasMaxLength(150);

            entity.Property(x => x.CreatedAt)
                .IsRequired();
        });
    }
}

