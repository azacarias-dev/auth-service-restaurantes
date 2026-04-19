using Microsoft.AspNetCore.Mvc;
using AuthService.Application.Interfaces;

namespace AuthService.Api.Controllers;

/// <summary>
/// Controlador de prueba para validación del servicio de mensajería.
/// </summary>
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
    /// Envía un correo electrónico de bienvenida como prueba técnica.
    /// </summary>
    /// <remarks>
    /// Este endpoint se utiliza para verificar que la configuración SMTP (Gmail) sea correcta.
    /// </remarks>
    /// <param name="email">Dirección de correo del destinatario.</param>
    /// <param name="name">Nombre del destinatario que aparecerá en el cuerpo del mensaje.</param>
    /// <returns>Mensaje de confirmación del envío exitoso.</returns>
    /// <response code="200">El correo fue encolado y enviado exitosamente.</response>
    /// <response code="400">Los datos proporcionados no son válidos.</response>
    /// <response code="500">Error en el servidor SMTP o credenciales inválidas.</response>
    [HttpPost("send-welcome")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> TestWelcome(string email, string name)
    {
        await _emailService.SendWelcomeEmailAsync(email, name);
        return Ok(new { message = $"¡Éxito! Correo enviado a {email}" });
    }
}