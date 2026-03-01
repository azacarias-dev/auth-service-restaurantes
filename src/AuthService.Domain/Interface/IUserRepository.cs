using AuthService.Domain.Entities;
namespace AuthService.Domain.Interfaces;

public interface IUserRepository{

    // Metodos de consuta
    Task<User> CreateAsync(User user);

    Task<User?> GetByIdAsync(string id);

    Task<User?> GetByEmailAsync(string email);

    Task<User?> GetBySurnameAsync(string surname);

    Task<User?> GetByEmailVerificationTokenAsync(string token);

    Task<User?> GetByPasswordResetTokenAsync(string token);

    Task<bool> ExistsByEmailAsync(string email);

    Task<bool> ExistsBySurnameAsync(string surname);

    Task<User> UpdateAsync(User user);
    
    Task<bool> DeleteAsync(string id);

    Task UpdateUserRoleAsync(string userId, string roleId);
}