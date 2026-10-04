using Microsoft.EntityFrameworkCore;
using ServePos.Infrastructure;

namespace ServePOS.API.Auth;

public class ActiveEventGuard(PosDbContext db)
{
    public Task<bool> HasActiveEvent(CancellationToken cancellationToken) =>
        db.Events.AnyAsync(x => x.IsActive && !x.IsCompleted, cancellationToken);

    public Task<bool> IsActiveEvent(int eventId, CancellationToken cancellationToken) =>
        db.Events.AnyAsync(
            x => x.Id == eventId && x.IsActive && !x.IsCompleted,
            cancellationToken);
}
