using Microsoft.EntityFrameworkCore;
using ServePos.Application.Repositoryies;
using ServePos.Domain.Entities;

namespace ServePos.Infrastructure.Repositories;

public class MenuRepository(PosDbContext _db) : Repository(_db), IMenuRepository
{
    public async Task<MenuItem?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await Context.MenuItems.FindAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<MenuItem>> GetAllAsync(CancellationToken cancellationToken)
    {
        var result = await Context.MenuItems
            .ToListAsync(cancellationToken);

        return result;
    }

    public async Task AddAsync(MenuItem product, CancellationToken cancellationToken)
    {
        Context.MenuItems.Add(product);
        await Context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(MenuItem product, CancellationToken cancellationToken)
    {
        Context.MenuItems.Update(product);
        await Context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var product = await Context.MenuItems.FindAsync(id, cancellationToken);
        if (product != null)
        {
            Context.MenuItems.Remove(product);
            await Context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<int> GetMaxSortIndexAsync(CancellationToken cancellationToken)
    {
        return await Context.MenuItems.MaxAsync(x => x.SortIndex, cancellationToken);
    }

    public async Task<MenuItem?> GetNextLowerSortIndex(int index, CancellationToken cancellationToken)
    {
        var result = await Context.MenuItems
            .Where(x => x.SortIndex < index)
            .OrderByDescending(x => x.SortIndex)
            .FirstOrDefaultAsync(cancellationToken);
        return result;
    }

    public Task<MenuItem?> GetNextHigherSortIndex(int index, CancellationToken cancellationToken)
    {
        var result = Context.MenuItems
            .Where(x => x.SortIndex > index)
            .OrderBy(x => x.SortIndex)
            .FirstOrDefaultAsync(cancellationToken);
        return result;
    }
}
