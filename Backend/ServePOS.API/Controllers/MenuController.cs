using Microsoft.AspNetCore.Mvc;
using ServePos.Application.Dtos.Menu;
using ServePos.Application.Services;

namespace ServePOS.API.Controllers;

[ApiController]
[Route("menu")]
public class MenuController(MenuService _service) : ControllerBase
{
    [HttpGet]
    public async Task<List<MenuItem>> GetMenu(CancellationToken cancellationToken)
    {
        var result = await _service.GetAll(cancellationToken);
        return result;
    }

    [HttpPost]
    [Route("item")]
    public async Task<IActionResult> AddMenuItem(MenuItemInsertInformation item, CancellationToken cancellationToken)
    {
        await _service.Add(item, cancellationToken);
        return Ok();
    }

    [HttpPut("item/{id}")]
    public async Task<IActionResult> Update(int id, MenuItemUpdateInformation item, CancellationToken cancellationToken)
    {
         await _service.Update(id, item, cancellationToken);
        return NoContent();
    }

    [HttpDelete("item/{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _service.Delete(id, cancellationToken);
        return NoContent();
    }

    [HttpPut("item/{id}/pushSortOrder")]
    public async Task<IActionResult> PushSortOrder(
        [FromRoute] int id,
        bool down, 
        CancellationToken cancellationToken)
    {
        await _service.PushSortOrder(id, down, cancellationToken);
        return Ok();
    }
}
