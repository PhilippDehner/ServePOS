using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServePos.Application.Dtos.Menu;
using ServePos.Application.Security;
using ServePos.Application.Services;
using ServePOS.API.Auth;

namespace ServePOS.API.Controllers;

[ApiController]
[Route("menu")]
public class MenuController(MenuService _service, ActiveEventGuard activeEventGuard) : ControllerBase
{
    [HttpGet]
    public async Task<List<MenuItem>> GetMenu(CancellationToken cancellationToken)
    {
        var result = await _service.GetAll(cancellationToken);
        return result;
    }

    [HttpPost]
    [Route("item")]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> AddMenuItem(MenuItemInsertInformation item, CancellationToken cancellationToken)
    {
        if (!await activeEventGuard.HasActiveEvent(cancellationToken))
        {
            return Conflict("No active event is available.");
        }

        await _service.Add(item, cancellationToken);
        return Ok();
    }

    [HttpPut("item/{id}")]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> Update(int id, MenuItemUpdateInformation item, CancellationToken cancellationToken)
    {
        if (!await activeEventGuard.HasActiveEvent(cancellationToken))
        {
            return Conflict("No active event is available.");
        }

        await _service.Update(id, item, cancellationToken);
        return NoContent();
    }

    [HttpDelete("item/{id}")]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        if (!await activeEventGuard.HasActiveEvent(cancellationToken))
        {
            return Conflict("No active event is available.");
        }

        await _service.Delete(id, cancellationToken);
        return NoContent();
    }

    [HttpPut("item/{id}/pushSortOrder")]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> PushSortOrder(
        [FromRoute] int id,
        bool down, 
        CancellationToken cancellationToken)
    {
        if (!await activeEventGuard.HasActiveEvent(cancellationToken))
        {
            return Conflict("No active event is available.");
        }

        await _service.PushSortOrder(id, down, cancellationToken);
        return Ok();
    }
}
