using AuthService.Domain.Entities;

namespace AuthService.Domain.Interfaces;

public interface IEmployeeRepository
{
    // Crear
    Task<Employees> CreateAsync(Employees employee);

    // Consultas
    Task<Employees?> GetByIdAsync(string id);

    Task<IEnumerable<Employees>> GetAllAsync();

    Task<Employees?> GetByDpiAsync(string dpi);

    // Validaciones
    Task<bool> ExistsByDpiAsync(string dpi);

    // Actualizar
    Task<Employees> UpdateAsync(Employees employee);

    Task<bool> UpdateStatusAsync(string id, string status);

    // Eliminar
    Task<bool> DeleteAsync(string id);
}