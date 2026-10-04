using Microsoft.EntityFrameworkCore;
using ServePos.Application.Repositoryies;
using ServePos.Domain.Entities;

namespace ServePos.Infrastructure.Repositories;

public class StaffRepository(PosDbContext _db) : Repository(_db), IStaffRepository
{
    public async Task AddAsync(string name, CancellationToken cancellationToken)
    {
        Context.Staff.Add(new Staff(name));
        await Context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<Staff>> GetAll(CancellationToken cancellationToken)
    {
        var result = await Context.Staff.ToListAsync(cancellationToken);
        return result;
    }

    public Task<Staff> GetByIdAsync(int staffId, CancellationToken cancellationToken)
    {
        var result = Context.Staff.FirstAsync(s => s.Id == staffId, cancellationToken);
        return result;
    }
}
