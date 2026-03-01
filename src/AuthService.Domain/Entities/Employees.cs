using System.ComponentModel.DataAnnotations;

namespace AuthService.Domain.Entities;

public class Employees
{
    [Key]
    [MaxLength(16)]
    public string Id { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es requerido.")]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es requerido.")]
    [MaxLength(50)]
    public string Surname { get; set; } = string.Empty;

    [Required(ErrorMessage = "El DPI es requerido.")]
    [MaxLength(20)]
    public string Dpi { get; set; } = string.Empty;

    [Required(ErrorMessage = "El puesto es requerido.")]
    [MaxLength(100)]
    public string Puesto { get; set; } = string.Empty;

    [Required(ErrorMessage = "El sueldo es requerido.")]
    [Range(0, double.MaxValue, ErrorMessage = "El sueldo debe ser mayor o igual a 0.")]
    public decimal Sueldo { get; set; }

    [Required(ErrorMessage = "El estado es requerido.")]
    [MaxLength(20)]
    public string Status { get; set; } = string.Empty;
}