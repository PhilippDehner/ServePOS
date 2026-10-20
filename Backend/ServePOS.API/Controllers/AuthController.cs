using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServePOS.API.Auth;

namespace ServePOS.API.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(AuthService authService) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var response = await authService.Login(request, cancellationToken);
        return response is null ? Unauthorized() : Ok(response);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var tokenId = User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value
            ?? User.FindFirst(ClaimTypes.SerialNumber)?.Value;
        var expiresAt = User.FindFirst(JwtRegisteredClaimNames.Exp)?.Value;
        if (tokenId is null || !long.TryParse(expiresAt, out var expiresAtSeconds))
        {
            return Unauthorized();
        }

        await authService.Logout(
            tokenId,
            DateTimeOffset.FromUnixTimeSeconds(expiresAtSeconds).UtcDateTime,
            cancellationToken);
        return NoContent();
    }
}
