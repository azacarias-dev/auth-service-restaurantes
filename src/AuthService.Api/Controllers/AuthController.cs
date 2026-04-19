using System;
using AuthService.Application.DTOs;
using AuthService.Application.DTOs.Email;
using AuthService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AuthService.Api.Controllers;

/// <summary>
/// Controlador encargado de la autenticacion, registro y gestion de perfiles de usuario.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>
    /// Obtiene el perfil del usuario autenticado actual.
    /// </summary>
    /// <remarks>Requiere un token JWT valido en el encabezado de autorizacion.</remarks>
    /// <returns>Objeto con los datos del perfil del usuario</returns>
    /// <response code="200">Retorna el perfil del usuario exitosamente</response>
    /// <response code="401">Si el token es invalido o no existe</response>
    /// <response code="404">Si el usuario no fue encontrado en la base de datos</response>
    [HttpGet("profile")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<object>> GetProfile()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "sub" || c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
        if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
        {
            return Unauthorized();
        }

        var user = await authService.GetUserByIdAsync(userIdClaim.Value);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(new
        {
            success = true,
            message = "Perfil obtenido exitosamente",
            data = user
        });
    }

    /// <summary>
    /// Obtiene el perfil de un usuario especifico mediante su ID.
    /// </summary>
    /// <param name="request">Objeto que contiene el ID del usuario a consultar</param>
    /// <returns>Datos del perfil del usuario solicitado</returns>
    /// <response code="200">Perfil obtenido exitosamente</response>
    /// <response code="400">Si el ID de usuario es nulo o vacio</response>
    /// <response code="404">Usuario no encontrado</response>
    [HttpPost("profile/by-id")]
    [EnableRateLimiting("ApiPolicy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<object>> GetProfileById([FromBody] GetProfileByIdDto request)
    {
        if (string.IsNullOrEmpty(request.UserId))
        {
            return BadRequest(new { success = false, message = "El userId es requerido" });
        }

        var user = await authService.GetUserByIdAsync(request.UserId);
        if (user == null)
        {
            return NotFound(new { success = false, message = "Usuario no encontrado" });
        }

        return Ok(new { success = true, message = "Perfil obtenido exitosamente", data = user });
    }

    /// <summary>
    /// Registra un nuevo usuario en el sistema.
    /// </summary>
    /// <param name="registerDto">Datos de registro y archivos multimedia del usuario</param>
    /// <returns>Resultado del registro y datos del usuario creado</returns>
    /// <response code="201">Usuario creado exitosamente</response>
    [HttpPost("register")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(10 * 1024 * 1024)] // 10MB limite
    [EnableRateLimiting("AuthPolicy")]
    [ProducesResponseType(typeof(RegisterResponseDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<RegisterResponseDto>> Register([FromForm] RegisterDto registerDto)
    {
        var result = await authService.RegisterAsync(registerDto);
        return StatusCode(201, result);
    }

    /// <summary>
    /// Inicia sesion y genera un token de acceso.
    /// </summary>
    /// <param name="loginDto">Credenciales de acceso (Email o Username y Password)</param>
    /// <returns>Token JWT e informacion basica del usuario</returns>
    /// <response code="200">Autenticacion exitosa</response>
    /// <response code="401">Credenciales incorrectas</response>
    [HttpPost("login")]
    [EnableRateLimiting("AuthPolicy")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        var result = await authService.LoginAsync(loginDto);
        return Ok(result);
    }

    /// <summary>
    /// Verifica la cuenta del usuario mediante un codigo enviado por correo.
    /// </summary>
    /// <param name="verifyEmailDto">Email y codigo de verificacion</param>
    /// <returns>Mensaje de exito o error en la verificacion</returns>
    /// <response code="200">Correo verificado correctamente</response>
    [HttpPost("verify-email")]
    [EnableRateLimiting("ApiPolicy")]
    [ProducesResponseType(typeof(EmailResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<EmailResponseDto>> VerifyEmail([FromBody] VerifyEmailDto verifyEmailDto)
    {
        var result = await authService.VerifyEmailAsync(verifyEmailDto);
        return Ok(result);
    }

    /// <summary>
    /// Reenvia el codigo de verificacion al correo del usuario.
    /// </summary>
    /// <param name="resendDto">Correo electronico del usuario</param>
    /// <returns>Confirmacion del reenvio del correo</returns>
    /// <response code="200">Correo reenviado exitosamente</response>
    /// <response code="400">Si el usuario ya esta verificado</response>
    /// <response code="404">Usuario no encontrado</response>
    /// <response code="503">Error en el servicio de correo</response>
    [HttpPost("resend-verification")]
    [EnableRateLimiting("AuthPolicy")]
    [ProducesResponseType(typeof(EmailResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<EmailResponseDto>> ResendVerification([FromBody] ResendVerificationDto resendDto)
    {
        var result = await authService.ResendVerificationEmailAsync(resendDto);

        if (!result.Success)
        {
            if (result.Message.Contains("no encontrado", StringComparison.OrdinalIgnoreCase)) return NotFound(result);
            if (result.Message.Contains("ya ha sido verificado", StringComparison.OrdinalIgnoreCase)) return BadRequest(result);
            return StatusCode(503, result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Solicita un restablecimiento de contraseña.
    /// </summary>
    /// <param name="forgotPasswordDto">Correo electronico para recibir el enlace de recuperacion</param>
    /// <returns>Confirmacion del envio del correo de recuperacion</returns>
    /// <response code="200">Instrucciones enviadas al correo (si existe)</response>
    /// <response code="503">Error al enviar el correo</response>
    [HttpPost("forgot-password")]
    [EnableRateLimiting("AuthPolicy")]
    [ProducesResponseType(typeof(EmailResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<EmailResponseDto>> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
    {
        var result = await authService.ForgotPasswordAsync(forgotPasswordDto);
        if (!result.Success) return StatusCode(503, result);
        return Ok(result);
    }

    /// <summary>
    /// Restablece la contraseña utilizando el token recibido por correo.
    /// </summary>
    /// <param name="resetPasswordDto">Nueva contraseña y token de validacion</param>
    /// <returns>Confirmacion del cambio de contraseña</returns>
    /// <response code="200">Contraseña actualizada exitosamente</response>
    /// <response code="400">Token invalido o expirado</response>
    [HttpPost("reset-password")]
    [EnableRateLimiting("AuthPolicy")]
    [ProducesResponseType(typeof(EmailResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EmailResponseDto>> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
    {
        var result = await authService.ResetPasswordAsync(resetPasswordDto);
        return Ok(result);
    }
}