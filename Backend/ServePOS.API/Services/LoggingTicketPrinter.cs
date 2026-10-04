using ServePos.Application.Services;
using ServePos.Domain.Entities;

namespace ServePOS.API.Services;

public class LoggingTicketPrinter(ILogger<LoggingTicketPrinter> logger) : ITicketPrinter
{
    public Task PrintAsync(Ticket ticket, bool isReprint, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "{PrintType} ticket {TicketId} for order {OrderId} at {Station}",
            isReprint ? "Reprinting" : "Printing",
            ticket.Id,
            ticket.OrderId,
            ticket.Station);
        return Task.CompletedTask;
    }
}
