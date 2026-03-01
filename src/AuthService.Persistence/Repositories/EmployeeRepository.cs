using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using AuthService.Persistence.Data;
using AuthService.Application.Services;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Persistence.Repositories;

public class EmployeeRepository(ApplicationDbContext context) : IEmployeeRepository
{
    public async Task<Employees> CreateAsync(Employees employee)
    {
        context.Employees.Add(employee);
        await context.SaveChangesAsync();
        return employee;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var employee = await GetByIdAsync(id);
        context.Employees.Remove(employee);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Employees>> GetAllAsync()
    {
        return await context.Employees.ToListAsync();
    }

    public async Task<Employees?> GetByDpiAsync(string dpi)
    {
        return await context.Employees
        .FirstOrDefaultAsync(e => EF.Functions.Like(e.Dpi, dpi));
    }

    public async Task<Employees?> GetByIdAsync(string id)
    {
        return await context.Employees.FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<bool> ExistsByDpiAsync(string dpi)
    {
        return await context.Employees.AnyAsync(e => EF.Functions.Like(e.Dpi, dpi));
    }

    public async Task<Employees> UpdateAsync(Employees employee)
    {
        await context.SaveChangesAsync();
        return await GetByIdAsync(employee.Id);
    }

    public async Task<bool> UpdateStatusAsync(string id, string status)
    {
        var existingEmployee = await context.Employees
        .Where(e => e.Id == id)
        .ToListAsync();

        context.Employees.RemoveRange(existingEmployee);

        var newEmployee = new Employees
        {
            Id = UuidGenerator.GenerateShortUUID(),
            Dpi = existingEmployee.First().Dpi,
            Name = existingEmployee.First().Name,
            Surname = existingEmployee.First().Surname,
            Puesto = existingEmployee.First().Puesto,
            Sueldo = existingEmployee.First().Sueldo,
            Status = status
        };
        context.Employees.Add(newEmployee);
        await context.SaveChangesAsync();
        return true;
    }
}