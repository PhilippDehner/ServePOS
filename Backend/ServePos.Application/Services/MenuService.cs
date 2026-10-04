using ServePos.Application.Dtos.Menu;
using ServePos.Application.Repositoryies;

namespace ServePos.Application.Services;

public class MenuService(IMenuRepository _repo)
{
    public async Task<MenuItem?> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _repo.GetByIdAsync(id, cancellationToken);
        return result == null ? null : new MenuItem
        {
            Id = result.Id,
            Name = result.Name,
            ShortName = result.ShortName,
            Type = result.Type,
            Price = result.Price,
            AvailableQuantity = result.AvailableQuantity,
            SoldQuantity = 0,
            SortIndex = result.SortIndex,
        };
    }

    public async Task<List<MenuItem>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _repo.GetAllAsync(cancellationToken);
        return result
            .Select(x => new MenuItem
            {
                Id = x.Id,
                Name = x.Name,
                ShortName = x.ShortName,
                Type = x.Type,
                Price = x.Price,
                AvailableQuantity = x.AvailableQuantity,
                SoldQuantity = 0,
                SortIndex = x.SortIndex
            })
            .OrderBy(x => x.SortIndex)
            .ToList();
    }

    public async Task Add(MenuItemInsertInformation insert, CancellationToken cancellationToken)
    {
        var currentMaxSortIndex = await _repo.GetMaxSortIndexAsync(cancellationToken);

        var menuItem = new Domain.Entities.MenuItem(insert.Name, insert.ShortName, insert.Type, insert.Price, insert.AvailableQuantity, currentMaxSortIndex + 1);
        await _repo.AddAsync(menuItem, cancellationToken);
    }

    public async Task Update(int id, MenuItemUpdateInformation update, CancellationToken cancellationToken)
    {
        var item = await _repo.GetByIdAsync(id, cancellationToken) ?? throw new Exception($"Menu item with id {id} not found.");
        item.Update(update.Name, update.ShortName, update.Type, update.Price, update.IsActive, update.AvailableQuantity);

        await _repo.SaveChangesAsync(cancellationToken);
    }

    public Task Delete(int id, CancellationToken cancellationToken) => _repo.DeleteAsync(id, cancellationToken);

    public async Task PushSortOrder(int id, bool down, CancellationToken cancellationToken)
    {
        var item = await _repo.GetByIdAsync(id, cancellationToken) ?? throw new Exception($"Menu item with id {id} not found.");

        var oldIndex = item.SortIndex;
        var newIndex = down ? oldIndex - 1 : oldIndex + 1;

        var otherItem = down
            ? await _repo.GetNextLowerSortIndex(oldIndex, cancellationToken)
            : await _repo.GetNextHigherSortIndex(oldIndex, cancellationToken);

    
        item.SetSortIndex(newIndex);
        otherItem?.SetSortIndex(oldIndex);

        await _repo.SaveChangesAsync(cancellationToken);
    }
}
