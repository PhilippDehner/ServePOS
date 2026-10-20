using Microsoft.EntityFrameworkCore;
using ServePos.Application.Security;
using ServePos.Domain.Entities;
using ServePos.Infrastructure;
using ServePOS.API.Auth;
using MenuItemKind = ServePos.Shared.MenuItemType;

namespace ServePOS.API.Services;

public class DevelopmentDataSeeder(PosDbContext db)
{
    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        if (await db.Events.AnyAsync(cancellationToken))
        {
            return;
        }

        var eventToSeed = new Event("ServePOS Testfest", DateTime.UtcNow.Date);
        eventToSeed.Activate();
        db.Events.Add(eventToSeed);
        await db.SaveChangesAsync(cancellationToken);

        var roles = await db.Roles.ToDictionaryAsync(x => x.Name, x => x.Id, cancellationToken);
        db.Users.AddRange(
            new User("bedienung1", PinHasher.Hash("1234"), roles[RoleNames.Cashier]),
            new User("bedienung2", PinHasher.Hash("1234"), roles[RoleNames.Cashier]),
            new User("kueche", PinHasher.Hash("1234"), roles[RoleNames.Kitchen]),
            new User("getraenke", PinHasher.Hash("1234"), roles[RoleNames.Drinks]));
        db.Staff.AddRange(
            new Staff("Anna Bedienung"),
            new Staff("Ben Bedienung"));

        var kitchen = new ServingStation("Küche", eventToSeed.Id);
        var drinks = new ServingStation("Getränke", eventToSeed.Id);
        db.ServingStations.AddRange(kitchen, drinks);

        db.Tables.AddRange(
            new Table("1", eventToSeed.Id),
            new Table("2", eventToSeed.Id),
            new Table("3", eventToSeed.Id),
            new Table("4", eventToSeed.Id),
            new Table("5", eventToSeed.Id),
            new Table("6", eventToSeed.Id),
            new Table("7", eventToSeed.Id),
            new Table("8", eventToSeed.Id));
        await db.SaveChangesAsync(cancellationToken);

        db.MenuItemTypes.AddRange(
            new MenuItemType("Speisen", kitchen.Id),
            new MenuItemType("Getränke", drinks.Id));

        db.MenuItems.AddRange(
            new MenuItem("Bratwurst", "Wurst", MenuItemKind.Food, 3.50m, 100, 1),
            new MenuItem("Pommes", null, MenuItemKind.Food, 3.00m, 100, 2),
            new MenuItem("Currywurst mit Pommes", "Currywurst", MenuItemKind.Food, 7.00m, 50, 3),
            new MenuItem("Wasser", null, MenuItemKind.Drink, 2.00m, 100, 4),
            new MenuItem("Apfelschorle", "Schorle", MenuItemKind.Drink, 2.50m, 100, 5),
            new MenuItem("Bier", null, MenuItemKind.Drink, 3.00m, 100, 6),
            new MenuItem("Kaffee", null, MenuItemKind.Drink, 2.00m, null, 7));

        await db.SaveChangesAsync(cancellationToken);
    }
}
