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
    public async Task<ActionResult<OrderReceipt>> Order(OrderInformation order, CancellationToken cancellationToken)
    { 
        if (!await activeEventGuard.HasActiveEvent(cancellationToken))
        {
            return Conflict("No active event is available.");
        }

        return Ok(await _service.Order(order, cancellationToken));
    }

    [HttpPost("order/{orderId:int}/cash-payment")]
    public async Task<ActionResult<CashPaymentResult>> PayCash(
        int orderId,
        CashPaymentInformation payment,
        CancellationToken cancellationToken)
    {
        if (!await activeEventGuard.HasActiveEvent(cancellationToken))
        {
            return Conflict("No active event is available.");
        }

        try
        {
            return Ok(await _service.PayCash(orderId, payment, cancellationToken));
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(exception.Message);
        }
    }
}
