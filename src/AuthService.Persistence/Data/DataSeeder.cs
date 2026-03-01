using AuthService.Domain.Entities;
using AuthService.Persistence.Data;
using AuthService.Application.Services;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Persistence.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        await context.Database.MigrateAsync();
        
        // CREAR ROLES
        if (!await context.Roles.AnyAsync())
        {
            var adminRole = new Role
            {
                Id = UuidGenerator.GenerateShortUUID(),
                Name = "Admin",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var userRole = new Role
            {
                Id = UuidGenerator.GenerateShortUUID(),
                Name = "User",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await context.Roles.AddRangeAsync(adminRole, userRole);
            await context.SaveChangesAsync();
        }


        // CREAR ADMIN COMO ROL
        var adminEmail = "ADMIN01@GMAIL.COM";

        var existingUser = await context.Users
            .FirstOrDefaultAsync(u => u.Email == adminEmail);

        if (existingUser != null)
            return;

        var adminUser = new User
        {
            Id = UuidGenerator.GenerateShortUUID(),
            Name = "ADMIN01",
            Surname = "ADMIN01",
            Email = adminEmail,
            Address = "SYSTEM ADDRESS",
            Phone = "00000000",
            IsActive = true
        };

        await context.Users.AddAsync(adminUser);
        await context.SaveChangesAsync();

        // ASIGNAR ROL ADMIN
        var adminRoleDb = await context.Roles
            .FirstOrDefaultAsync(r => r.Name == "Admin");

        if (adminRoleDb != null)
        {
            var userRole = new UserRole
            {
                Id = UuidGenerator.GenerateShortUUID(),
                UserId = adminUser.Id,
                RoleId = adminRoleDb.Id,
                AssignedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await context.UserRoles.AddAsync(userRole);
        }

        // CREAR USER EMAIL
        var userEmail = new UserEmail
        {
            Id = UuidGenerator.GenerateShortUUID(),
            UserId = adminUser.Id,
            EmailVerified = true
        };

        await context.UserEmails.AddAsync(userEmail);

        // CREAR PASSWORD RESET
        var passwordReset = new UserPasswordReset
        {
            Id = UuidGenerator.GenerateShortUUID(),
            UserId = adminUser.Id
        };

        await context.UserPasswordResets.AddAsync(passwordReset);

        await context.SaveChangesAsync();
    }
}