using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using AuthService.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Persistence.Repositories;

public class RoleRepository(ApplicationDbContext context) : IRoleRepository
{
    public async Task<Role?> GetByNameAsync(string name)
    {
        return await context.Roles
        .Include(r => r.UserRoles)
        .FirstOrDefaultAsync(r => EF.Functions.Like(r.Name, name));
    }

    public async Task<int> CountUsersInRoleAsync(string roleName)
    {
        return await context.UserRoles
        .Where(ur => ur.Role.Name == roleName)
        .CountAsync();
    }

    public async Task<IReadOnlyList<User>> GetUsersByRoleAsync(string roleName)
    {
        // 1. Empezamos directamente desde los Usuarios
        var users = await context.Users
        .Include(u => u.UserEmail)   // Incluye el email directamente
        .Include(u => u.UserRoles)   // Incluye la tabla intermedia de roles
            .ThenInclude(ur => ur.Role) // Incluye el detalle del rol de esa intermedia
        .Where(u => u.UserRoles.Any(ur => ur.Role.Name == roleName)) // 2. Filtramos los usuarios que pertenezcan a ese rol
        .ToListAsync();

        // 3. Retornamos la lista directamente (C# hace el cast implícito a IReadOnlyList)
        return users;

    }

    public async Task<IReadOnlyList<string>> GetUserRoleNameAsync(string userId)
    {
        return await context.UserRoles
        .Where(ur => ur.UserId == userId)
        .Select(ur => ur.Role.Name)
        .ToListAsync()
        .ContinueWith(t => (IReadOnlyList<string>)t.Result);
    }
}