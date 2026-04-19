using System.ComponentModel.DataAnnotations;

namespace AuthService.Domain.Entities;

public class User
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

    [Required(ErrorMessage = "La contraseña es requerida.")]
    [MaxLength(255)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es requerido.")]
    [EmailAddress(ErrorMessage = "Formato de correo invalido.")]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La direccion es requerida.")]
    [MaxLength(150)]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "El telefono es requerido.")]
    [Phone(ErrorMessage = "Formato de telefono invalido.")]
    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "El estado es requerido.")]
    public bool IsActive { get; set; } = true;

    [Required]
    public DateTime CreatedAt {get; set;}

    [Required]
    public DateTime UpdatedAt {get; set;}

    public ICollection<UserRole> UserRoles { get; set; } = [];
    public UserEmail UserEmail { get; set; } = null!;
    public UserPasswordReset UserPasswordReset { get; set; } = null!;
}