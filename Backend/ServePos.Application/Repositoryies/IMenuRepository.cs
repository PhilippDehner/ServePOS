using ServePos.Domain.Entities;

namespace ServePos.Application.Repositoryies;

public interface IMenuRepository
{
    Task SaveChangesAsync(CancellationToken cancellationToken);
    Task<MenuItem?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<IEnumerable<MenuItem>> GetAllAsync(CancellationToken cancellationToken);
    Task AddAsync(MenuItem menuItem, CancellationToken cancellationToken);
    Task UpdateAsync(MenuItem menuItem, CancellationToken cancellationToken);
    Task DeleteAsync(int id, CancellationToken cancellationToken);
    Task<int> GetMaxSortIndexAsync(CancellationToken cancellationToken);
    Task<MenuItem?> GetNextLowerSortIndex(int index, CancellationToken cancellationToken);
    Task<MenuItem?> GetNextHigherSortIndex(int index, CancellationToken cancellationToken);
}
