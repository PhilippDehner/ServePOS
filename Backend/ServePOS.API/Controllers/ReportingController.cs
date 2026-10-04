using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServePos.Application.Security;
using ServePos.Infrastructure;

namespace ServePOS.API.Controllers;

[ApiController]
[Authorize(Roles = RoleNames.Admin)]
[Route("reports/events")]
public class ReportingController(PosDbContext db) : ControllerBase
{
    [HttpGet("{eventId:int}")]
    public async Task<ActionResult<EventReport>> Get(int eventId, CancellationToken cancellationToken)
    {
        if (!await db.Events.AnyAsync(x => x.Id == eventId, cancellationToken))
        {
            return NotFound();
        }

        var items = await db.Set<ServePos.Domain.Entities.OrderItem>()
            .Where(x => x.Order.EventId == eventId && x.Order.IsPaid)
            .Include(x => x.MenuItem)
            .GroupBy(x => new { x.MenuItemId, x.MenuItem.Name })
            .Select(x => new ItemSalesReport
            {
                MenuItemId = x.Key.MenuItemId,
                Name = x.Key.Name,
                Quantity = x.Count(y => !y.IsCancelled),
                GrossAmount = x.Sum(y => y.UnitPrice ?? 0),
                RefundAmount = db.OrderCancellations
                    .Where(c => x.Select(i => i.Id).Contains(c.OrderItemId))
                    .Sum(c => (decimal?)c.RefundAmount) ?? 0,
            })
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        var orderDetails = await db.Orders
            .Where(x => x.EventId == eventId && x.IsPaid)
            .Include(x => x.CashPayment)
            .Include(x => x.Items)
            .Select(x => new OrderReport
            {
                OrderId = x.Id,
                TableId = x.TableId,
                PaidAt = x.CashPayment!.CreatedAt,
                GrossAmount = x.CashPayment.TotalAmount,
                RefundAmount = db.OrderCancellations
                    .Where(c => x.Items.Select(i => i.Id).Contains(c.OrderItemId))
                    .Sum(c => (decimal?)c.RefundAmount) ?? 0,
            })
            .OrderBy(x => x.PaidAt)
            .ToListAsync(cancellationToken);

        var refunds = orderDetails.Sum(x => x.RefundAmount);
        return Ok(new EventReport
        {
            EventId = eventId,
            GrossRevenue = orderDetails.Sum(x => x.GrossAmount),
            RefundAmount = refunds,
            NetRevenue = orderDetails.Sum(x => x.GrossAmount) - refunds,
            Orders = orderDetails,
            ItemSales = items,
        });
    }
}

public class EventReport
{
    public required int EventId { get; init; }
    public required decimal GrossRevenue { get; init; }
    public required decimal RefundAmount { get; init; }
    public required decimal NetRevenue { get; init; }
    public required List<OrderReport> Orders { get; init; }
    public required List<ItemSalesReport> ItemSales { get; init; }
}

public class OrderReport
{
    public required int OrderId { get; init; }
    public required string TableId { get; init; }
    public required DateTime PaidAt { get; init; }
    public required decimal GrossAmount { get; init; }
    public required decimal RefundAmount { get; init; }
}

public class ItemSalesReport
{
    public required int MenuItemId { get; init; }
    public required string Name { get; init; }
    public required int Quantity { get; init; }
    public required decimal GrossAmount { get; init; }
    public required decimal RefundAmount { get; init; }
}
