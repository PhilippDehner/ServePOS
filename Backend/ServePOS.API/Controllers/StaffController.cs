using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServePos.Application.Dtos.Staff;
using ServePos.Application.Security;
using ServePos.Application.Services;
using ServePOS.API.Auth;

namespace ServePOS.API.Controllers;

[ApiController]
[Route("staff")]
public class StaffController(StaffService _service, ActiveEventGuard activeEventGuard) : ControllerBase
{
    [HttpGet]
    public async Task<List<StaffInfo>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _service.GetAll(cancellationToken);
        return result;
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> AddStaff(StaffUpsert item, CancellationToken cancellationToken)
    {
        if (!await activeEventGuard.HasActiveEvent(cancellationToken))
        {
            return Conflict("No active event is available.");
        }

        await _service.Add(item, cancellationToken);
        return Ok();
    }

    [HttpPut("{id}")]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> Update(int id, StaffUpsert item, CancellationToken cancellationToken)
    {
        if (!await activeEventGuard.HasActiveEvent(cancellationToken))
        {
            return Conflict("No active event is available.");
        }

        await _service.Update(id, item, cancellationToken);
        return NoContent();
    }
}
