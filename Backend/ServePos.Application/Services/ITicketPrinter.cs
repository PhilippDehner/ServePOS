using ServePos.Domain.Entities;

namespace ServePos.Application.Services;

public interface ITicketPrinter
{
    Task PrintAsync(Ticket ticket, bool isReprint, CancellationToken cancellationToken);
}
