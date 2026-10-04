using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServePos.Application.Security;
using ServePos.Application.Services;
using ServePos.Domain.Entities;
using ServePos.Infrastructure;

namespace ServePOS.API.Controllers;

[ApiController]
[Authorize(Roles = RoleNames.Admin + "," + RoleNames.Kitchen + "," + RoleNames.Drinks)]
[Route("tickets")]
public class TicketController(PosDbContext db, ITicketPrinter ticketPrinter) : ControllerBase
{
    [HttpGet("open/{station}")]
    public Task<List<TicketInfo>> GetOpen(TicketStation station, CancellationToken cancellationToken) =>
        db.Tickets
            .Where(x => x.Station == station && x.Items.Any(i => !i.OrderItem.IsServed))
            .Include(x => x.Order)
            .Include(x => x.Items).ThenInclude(x => x.OrderItem).ThenInclude(x => x.MenuItem)
            .OrderBy(x => x.CreatedAt)
            .Select(x => new TicketInfo
            {
                Id = x.Id,
                OrderId = x.OrderId,
                TableId = x.Order.TableId,
                Station = x.Station,
                PrintCount = x.PrintCount,
                Items = x.Items.Where(i => !i.OrderItem.IsServed).Select(i => new TicketItemInfo
                {
                    OrderItemId = i.OrderItemId,
                    Name = i.OrderItem.MenuItem.Name,
                    SpecialInstructions = i.OrderItem.SpecialInstructions,
                }).ToList(),
            })
            .ToListAsync(cancellationToken);

    [HttpPost("{ticketId:int}/issue")]
    public async Task<IActionResult> Issue(int ticketId, [FromBody] TicketIssue request, CancellationToken cancellationToken)
    {
        var ticket = await db.Tickets.Include(x => x.Items).ThenInclude(x => x.OrderItem)
            .SingleOrDefaultAsync(x => x.Id == ticketId, cancellationToken);
        if (ticket is null)
        {
            return NotFound();
        }

        foreach (var item in ticket.Items.Where(x => request.OrderItemIds.Contains(x.OrderItemId)))
        {
            item.OrderItem.MarkServed();
        }

        await db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpPost("{ticketId:int}/reprint")]
    public async Task<ActionResult<TicketInfo>> Reprint(int ticketId, CancellationToken cancellationToken)
    {
        var ticket = await db.Tickets.SingleOrDefaultAsync(x => x.Id == ticketId, cancellationToken);
        if (ticket is null)
        {
            return NotFound();
        }

        ticket.MarkReprinted();
        await db.SaveChangesAsync(cancellationToken);
        await ticketPrinter.PrintAsync(ticket, true, cancellationToken);
        return Ok(new TicketInfo { Id = ticket.Id, OrderId = ticket.OrderId, TableId = "", Station = ticket.Station, PrintCount = ticket.PrintCount, Items = [] });
    }
}

public class TicketIssue
{
    public required List<int> OrderItemIds { get; init; }
}

public class TicketInfo
{
    public required int Id { get; init; }
    public required int OrderId { get; init; }
    public required string TableId { get; init; }
    public required TicketStation Station { get; init; }
    public required int PrintCount { get; init; }
    public required List<TicketItemInfo> Items { get; init; }
}

public class TicketItemInfo
{
    public required int OrderItemId { get; init; }
    public required string Name { get; init; }
    public string? SpecialInstructions { get; init; }
}
