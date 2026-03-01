using AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Persistence.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<UserEmail> UserEmails { get; set; }
    public DbSet<UserPasswordReset> UserPasswordResets { get; set; }
    public DbSet<Employees> Employees { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // CONVERTIR A SNAKE_CASE
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            entity.SetTableName(ToSnakeCase(entity.GetTableName()!));

            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(ToSnakeCase(property.Name));
            }
        }

        // USER
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.Email).IsUnique();

            entity.HasMany(u => u.UserRoles)
                  .WithOne(ur => ur.User)
                  .HasForeignKey(ur => ur.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(u => u.UserEmail)
                  .WithOne(ue => ue.User)
                  .HasForeignKey<UserEmail>(ue => ue.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(u => u.UserPasswordReset)
                  .WithOne(upr => upr.User)
                  .HasForeignKey<UserPasswordReset>(upr => upr.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ROLE
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Name).IsUnique();
        });

        // USER ROLE
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => new { e.UserId, e.RoleId })
                  .IsUnique();
        });

        // EMPLOYEES
        modelBuilder.Entity<Employees>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Dpi).IsUnique();
        });
    }

    private static string ToSnakeCase(string input)
    {
        return string.Concat(
            input.Select((x, i) =>
                i > 0 && char.IsUpper(x)
                    ? "_" + x.ToString().ToLower()
                    : x.ToString().ToLower())
        );
    }
}