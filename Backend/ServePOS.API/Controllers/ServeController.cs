using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServePos.Application.Dtos.Order;
using ServePos.Application.Security;
using ServePos.Application.Services;
using ServePOS.API.Auth;

namespace ServePOS.API.Controllers;

[ApiController]
[Authorize(Roles = RoleNames.Admin + "," + RoleNames.Cashier)]
[Route("serve")]
public class ServeController(OrderService _service, ActiveEventGuard activeEventGuard) : ControllerBase
{
    [HttpPost]
    [Route("order")]
    public async Task<IActionResult> Order(OrderInformation order, CancellationToken cancellationToken)
    { 
        if (!await activeEventGuard.HasActiveEvent(cancellationToken))
        {
            return Conflict("No active event is available.");
        }

        await _service.Order(order, cancellationToken);
        return Ok();
    }
}
