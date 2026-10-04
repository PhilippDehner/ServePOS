using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ServePos.Application.Security;
using ServePos.Domain.Entities;
using ServePos.Infrastructure;

namespace ServePOS.API.Auth;

public class Bootstrapper(PosDbContext db, IOptions<BootstrapOptions> options)
{
    public async Task Initialize(CancellationToken cancellationToken)
    {
        foreach (var roleName in RoleNames.All)
        {
            if (!await db.Roles.AnyAsync(x => x.Name == roleName, cancellationToken))
            {
                db.Roles.Add(new Role(roleName));
            }
        }

        await db.SaveChangesAsync(cancellationToken);

        if (await db.Users.AnyAsync(cancellationToken))
        {
            return;
        }

        var bootstrap = options.Value;
        if (string.IsNullOrWhiteSpace(bootstrap.Username) || string.IsNullOrWhiteSpace(bootstrap.Pin))
        {
            throw new InvalidOperationException(
                "BootstrapAdmin:Username and BootstrapAdmin:Pin must be configured before the first startup.");
        }

        var adminRole = await db.Roles.SingleAsync(x => x.Name == RoleNames.Admin, cancellationToken);
        db.Users.Add(new User(bootstrap.Username, PinHasher.Hash(bootstrap.Pin), adminRole.Id));
        await db.SaveChangesAsync(cancellationToken);
    }
}
