using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServePos.Application.Security;
using ServePos.Domain.Entities;
using ServePos.Infrastructure;
using ServePOS.API.Auth;

namespace ServePOS.API.Controllers;

[ApiController]
[Authorize(Roles = RoleNames.Admin)]
[Route("administration")]
public class AdministrationController(PosDbContext db, ActiveEventGuard activeEventGuard) : ControllerBase
{
    [HttpGet("events")]
    public Task<List<EventInfo>> GetEvents(CancellationToken cancellationToken) =>
        db.Events.OrderByDescending(x => x.StartsAt).Select(x => new EventInfo
        {
            Id = x.Id, Name = x.Name, StartsAt = x.StartsAt, IsActive = x.IsActive, IsCompleted = x.IsCompleted
        }).ToListAsync(cancellationToken);

    [HttpPost("events")]
    public async Task<ActionResult<EventInfo>> CreateEvent(EventUpsert request, CancellationToken cancellationToken)
    {
        var @event = new Event(request.Name, request.StartsAt);
        db.Events.Add(@event);
        await db.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetEvents), new EventInfo
        {
            Id = @event.Id, Name = @event.Name, StartsAt = @event.StartsAt, IsActive = @event.IsActive, IsCompleted = @event.IsCompleted
        });
    }

    [HttpPost("events/{id:int}/activate")]
    public async Task<IActionResult> ActivateEvent(int id, CancellationToken cancellationToken)
    {
        var @event = await db.Events.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (@event is null || @event.IsCompleted)
        {
            return NotFound();
        }

        var activeEvent = await db.Events.SingleOrDefaultAsync(x => x.IsActive, cancellationToken);
        activeEvent?.Deactivate();
        @event.Activate();
        await db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpPost("events/{id:int}/complete")]
    public async Task<IActionResult> CompleteEvent(int id, CancellationToken cancellationToken)
    {
        var @event = await db.Events.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (@event is null)
        {
            return NotFound();
        }

        @event.Complete();
        await db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpGet("roles")]
    public Task<List<Role>> GetRoles(CancellationToken cancellationToken) =>
        db.Roles.OrderBy(x => x.Name).ToListAsync(cancellationToken);

    [HttpGet("users")]
    public Task<List<UserInfo>> GetUsers(CancellationToken cancellationToken) =>
        db.Users.Include(x => x.Role).OrderBy(x => x.Username).Select(x => new UserInfo
        {
            Id = x.Id, Username = x.Username, RoleId = x.RoleId, RoleName = x.Role.Name, IsActive = x.IsActive
        }).ToListAsync(cancellationToken);

    [HttpPost("users")]
    public async Task<ActionResult<UserInfo>> CreateUser(UserUpsert request, CancellationToken cancellationToken)
    {
        if (request.Pin.Length < 4 || !await db.Roles.AnyAsync(x => x.Id == request.RoleId, cancellationToken))
        {
            return BadRequest();
        }

        var user = new User(request.Username, PinHasher.Hash(request.Pin), request.RoleId);
        db.Users.Add(user);
        await db.SaveChangesAsync(cancellationToken);
        var role = await db.Roles.SingleAsync(x => x.Id == user.RoleId, cancellationToken);
        return Ok(new UserInfo
        {
            Id = user.Id, Username = user.Username, RoleId = user.RoleId, RoleName = role.Name, IsActive = user.IsActive
        });
    }

    [HttpPost("users/{id:int}/deactivate")]
    public async Task<IActionResult> DeactivateUser(int id, CancellationToken cancellationToken)
    {
        var user = await db.Users.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (user is null)
        {
            return NotFound();
        }

        user.SetActive(false);
        await db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpGet("events/{eventId:int}/tables")]
    public Task<List<Table>> GetTables(int eventId, CancellationToken cancellationToken) =>
        db.Tables.Where(x => x.EventId == eventId).OrderBy(x => x.Name).ToListAsync(cancellationToken);

    [HttpPost("events/{eventId:int}/tables")]
    public async Task<IActionResult> CreateTable(int eventId, NamedResourceUpsert request, CancellationToken cancellationToken)
    {
        if (!await activeEventGuard.IsActiveEvent(eventId, cancellationToken))
        {
            return Conflict("The event is not active.");
        }

        db.Tables.Add(new Table(request.Name, eventId));
        await db.SaveChangesAsync(cancellationToken);
        return Ok();
    }

    [HttpGet("serving-stations")]
    public Task<List<ServingStation>> GetServingStations(CancellationToken cancellationToken) =>
        db.ServingStations.OrderBy(x => x.Name).ToListAsync(cancellationToken);

    [HttpPost("serving-stations")]
    public async Task<IActionResult> CreateServingStation(ServingStationUpsert request, CancellationToken cancellationToken)
    {
        if (!await activeEventGuard.IsActiveEvent(request.EventId, cancellationToken))
        {
            return Conflict("The event is not active.");
        }

        db.ServingStations.Add(new ServingStation(request.Name, request.EventId));
        await db.SaveChangesAsync(cancellationToken);
        return Ok();
    }

    [HttpGet("menu-item-types")]
    public Task<List<MenuItemType>> GetMenuItemTypes(CancellationToken cancellationToken) =>
        db.MenuItemTypes.Include(x => x.ServingStation).OrderBy(x => x.Name).ToListAsync(cancellationToken);

    [HttpPost("menu-item-types")]
    public async Task<IActionResult> CreateMenuItemType(MenuItemTypeUpsert request, CancellationToken cancellationToken)
    {
        var stationEventId = await db.ServingStations
            .Where(x => x.Id == request.ServingStationId)
            .Select(x => (int?)x.EventId)
            .SingleOrDefaultAsync(cancellationToken);
        if (stationEventId is null || !await activeEventGuard.IsActiveEvent(stationEventId.Value, cancellationToken))
        {
            return Conflict("The serving station does not belong to the active event.");
        }

        db.MenuItemTypes.Add(new MenuItemType(request.Name, request.ServingStationId));
        await db.SaveChangesAsync(cancellationToken);
        return Ok();
    }
}
