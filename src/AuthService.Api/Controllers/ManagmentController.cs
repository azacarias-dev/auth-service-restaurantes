using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AuthService.Api.Controllers;
[ApiController]
[Route("api/v1/[controller]")]
public class UsersController(IUserManagementService userManagementService) : ControllerBase
{
    /// <summary>
    /// Obtiene la lista de todos los administradores registrados.
    /// </summary>
    /// <response code="200">Lista de administradores obtenida exitosamente.</response>
    [HttpGet("admins")]
    [EnableRateLimiting("ApiPolicy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<object>> GetAdmins()
    {
        var admins = await userManagementService.GetAdminsAsync();

        return Ok(new
        {
            success = true,
            message = "Administradores obtenidos exitosamente",
            data = admins
        });
    }

    /// <summary>
    /// Obtiene la lista de todos los usuarios registrados.
    /// </summary>
    /// <response code="200">Lista de usuarios obtenida exitosamente.</response>
    [HttpGet("users")]
    [EnableRateLimiting("ApiPolicy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<object>> GetUsuarios()
    {
        var usuarios = await userManagementService.GetUsuariosAsync();

        return Ok(new
        {
            success = true,
            message = "Usuarios obtenidos exitosamente",
            data = usuarios
        });
    }
}