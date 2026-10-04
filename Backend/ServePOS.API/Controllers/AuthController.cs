using Microsoft.AspNetCore.Mvc;
using ServePOS.API.Auth;

namespace ServePOS.API.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(AuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var response = await authService.Login(request, cancellationToken);
        return response is null ? Unauthorized() : Ok(response);
    }
}
