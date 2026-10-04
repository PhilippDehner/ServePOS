using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServePos.Application.Security;
using ServePos.Domain.Entities;
using ServePos.Infrastructure;
using ServePOS.API.Auth;

namespace ServePOS.API.Controllers;

[ApiController]
[Authorize(Roles = RoleNames.Admin + "," + RoleNames.Cashier)]
[Route("orders")]
public class CancellationController(PosDbContext db, ActiveEventGuard activeEventGuard) : ControllerBase
{
    [HttpPost("{orderId:int}/items/{orderItemId:int}/cancel")]
    public async Task<IActionResult> Cancel(
        int orderId,
        int orderItemId,
        OrderCancellationRequest request,
        CancellationToken cancellationToken)
    {
        if (!await activeEventGuard.HasActiveEvent(cancellationToken))
        {
            return Conflict("No active event is available.");
        }

        if (string.IsNullOrWhiteSpace(request.Reason) || string.IsNullOrWhiteSpace(request.DeviceId))
        {
            return BadRequest("A reason and device identifier are required.");
        }

        var item = await db.Set<OrderItem>()
            .Include(x => x.Order)
            .Include(x => x.MenuItem)
            .SingleOrDefaultAsync(x => x.Id == orderItemId && x.OrderId == orderId, cancellationToken);
        if (item is null || !item.Order.IsPaid || item.Order.EventId is null)
        {
            return NotFound();
        }

        if (item.IsCancelled)
        {
            return Conflict("The order item has already been cancelled.");
        }

        if (!await db.Staff.AnyAsync(x => x.Id == request.StaffId, cancellationToken))
        {
            return BadRequest("The staff member does not exist.");
        }

        item.Cancel();
        if (!item.IsServed)
        {
            item.MenuItem.IncreaseAvailableQuantity();
        }

        var refundAmount = item.UnitPrice ?? item.MenuItem.Price;
        db.OrderCancellations.Add(new OrderCancellation(item.Id, request.Reason.Trim(), refundAmount, request.StaffId, request.DeviceId.Trim()));
        db.AuditEvents.Add(new AuditEvent(
            "OrderItemCancelled",
            nameof(OrderItem),
            item.Id,
            request.StaffId,
            request.DeviceId.Trim(),
            request.Reason.Trim()));
        await db.SaveChangesAsync(cancellationToken);
        return Ok(new { refundAmount });
    }
}

public class OrderCancellationRequest
{
    public required string Reason { get; init; }
    public required int StaffId { get; init; }
    public required string DeviceId { get; init; }
}
