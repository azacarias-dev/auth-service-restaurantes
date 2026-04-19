using Microsoft.AspNetCore.Mvc;
using AuthService.Application.Interfaces;
 
namespace AuthService.Api.Controllers;
 /// <summary>
/// Controlador de prueba para enviar correos electrónicos.
[ApiController]
[Route("api/[controller]")]
public class EmailTestController : ControllerBase
{
    private readonly IEmailService _emailService;
 
    public EmailTestController(IEmailService emailService)
    {
        _emailService = emailService;
    }
 
    /// <summary>
    /// Endpoint de prueba para enviar un correo de bienvenida.
    /// </summary>
    /// <param name="email">Dirección de correo del destinatario.</param>
    /// <param name="name">Nombre del destinatario.</param>
    /// <returns>Resultado de la operación.</returns>
    /// response code="200">Correo enviado exitosamente.</response>
    /// response code="400">Solicitud inválida.</response>
    /// response code="500">Error interno del servidor.</response>
    [HttpPost("send-welcome")]
    public async Task<IActionResult> TestWelcome(string email, string name)
    {
        await _emailService.SendWelcomeEmailAsync(email, name);
        return Ok(new { message = $"¡Éxito! Correo enviado a {email}" });
    }
}
 