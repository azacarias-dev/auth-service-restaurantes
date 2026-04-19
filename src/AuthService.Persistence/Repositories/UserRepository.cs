using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using AuthService.Persistence.Data;
using AuthService.Application.Services;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Persistence.Repositories;

public class UserRepository(ApplicationDbContext context) : IUserRepository
{
    public async Task<User?> GetByIdAsync(string id)
    {
        var user = await context.Users
        .Include(u => u.UserRoles)
        .ThenInclude(ur => ur.Role)
        .Include(u => u.UserEmail)
        .Include(u => u.UserPasswordReset)
        .FirstOrDefaultAsync(u => u.Id == id);
        return user;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await context.Users
        .Include(u => u.UserEmail)
        .Include(u => u.UserPasswordReset)
        .Include(u => u.UserRoles)
        .ThenInclude(ur => ur.Role)
        .FirstOrDefaultAsync(u => EF.Functions.Like(u.Email, email));
    }

    public async Task<User?> GetByEmailVerificationTokenAsync(string token)
    {
        return await context.Users
        .Include(u => u.UserEmail)
        .Include(u => u.UserPasswordReset)
        .Include(u => u.UserRoles)
        .ThenInclude(ur => ur.Role)
        .FirstOrDefaultAsync(u => u.UserEmail != null && u.UserEmail.EmailVerificationToken == token);
    }


    public async Task<User?> GetByPasswordResetTokenAsync(string token)
    {
        return await context.Users
        .Include(u => u.UserEmail)
        .Include(u => u.UserPasswordReset)
        .Include(u => u.UserRoles)
        .ThenInclude(ur => ur.Role)
        .FirstOrDefaultAsync(u => u.UserPasswordReset != null && u.UserPasswordReset.PasswordResetToken == token);
    }

    public async Task<User?> GetByNameAsync(string name)
    {
        return await context.Users
        .Include(u => u.UserEmail)
        .Include(u => u.UserPasswordReset)
        .Include(u => u.UserRoles)
        .ThenInclude(ur => ur.Role)
        .FirstOrDefaultAsync(u => EF.Functions.Like(u.Name, name));
    }

    public async Task<User?> GetBySurnameAsync(string surname)
    {
        return await context.Users
        .Include(u => u.UserEmail)
        .Include(u => u.UserPasswordReset)
        .Include(u => u.UserRoles)
        .ThenInclude(ur => ur.Role)
        .FirstOrDefaultAsync(u => EF.Functions.Like(u.Surname, surname));
    }

    public async Task<User> CreateAsync(User user)
    {
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return await GetByIdAsync(user.Id);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var user = await GetByIdAsync(id);
        context.Users.Remove(user);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await context.Users.AnyAsync(u => EF.Functions.Like(u.Email, email));
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await context.Users.AnyAsync(u => EF.Functions.Like(u.Name, name));
    }

    public async Task<bool> ExistsBySurnameAsync(string surname)
    {
        return await context.Users.AnyAsync(u => EF.Functions.Like(u.Surname, surname));
    }

    public async Task UpdateUserRoleAsync(string userId, string roleId)
    {
       var existingUserRole = await context.UserRoles
       .Where(ur => ur.UserId == userId)
       .ToListAsync();

       context.UserRoles.RemoveRange(existingUserRole);

       var newUserRole = new UserRole
       {
        Id = UuidGenerator.GenerateShortUUID(),
        UserId = userId,
        RoleId = roleId,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
       };
        context.UserRoles.Add(newUserRole);
        await context.SaveChangesAsync();
    }

    public async Task<User> UpdateAsync(User user)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync();
        return user;
    }
}
