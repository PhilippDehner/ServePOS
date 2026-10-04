using Microsoft.AspNetCore.Mvc;
using ServePos.Application.Dtos.Order;
using ServePos.Application.Services;

namespace ServePOS.API.Controllers;

[ApiController]
[Route("serve")]
public class ServeController(OrderService _service) : ControllerBase
{
    [HttpPost]
    [Route("order")]
    public async Task<IActionResult> Order(OrderInformation order, CancellationToken cancellationToken)
    { 
        await _service.Order(order, cancellationToken);
        return Ok();
    }
}
