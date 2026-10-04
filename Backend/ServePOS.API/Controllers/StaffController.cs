using Microsoft.AspNetCore.Mvc;
using ServePos.Application.Dtos.Staff;
using ServePos.Application.Services;

namespace ServePOS.API.Controllers;

[ApiController]
[Route("staff")]
public class StaffController(StaffService _service) : ControllerBase
{
    [HttpGet]
    public async Task<List<StaffInfo>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _service.GetAll(cancellationToken);
        return result;
    }

    [HttpPost]
    public async Task<IActionResult> AddStaff(StaffUpsert item, CancellationToken cancellationToken)
    {
        await _service.Add(item, cancellationToken);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, StaffUpsert item, CancellationToken cancellationToken)
    {
        await _service.Update(id, item, cancellationToken);
        return NoContent();
    }
}
