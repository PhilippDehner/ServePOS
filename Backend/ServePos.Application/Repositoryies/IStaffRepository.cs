using ServePos.Domain.Entities;

namespace ServePos.Application.Repositoryies;

public interface IStaffRepository
{
    Task AddAsync(string name, CancellationToken cancellationToken);
    Task<List<Staff>> GetAll(CancellationToken cancellationToken);
    Task<Staff> GetByIdAsync(int staffId, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
