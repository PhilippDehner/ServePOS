using ServePos.Application.Dtos.Menu;
using ServePos.Application.Dtos.Staff;
using ServePos.Application.Repositoryies;

namespace ServePos.Application.Services;

public class StaffService(IStaffRepository _repository)
{
    public async Task Add(StaffUpsert item, CancellationToken cancellationToken)
    {
        await _repository.AddAsync(item.Name, cancellationToken);
    }

    public async Task<List<StaffInfo>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _repository.GetAll(cancellationToken);
        return result.Select(x => new StaffInfo
        {
            Id = x.Id,
            Name = x.Name,
            LastOperation = x.LastOperation
        }).ToList();
    }

    public async Task Update(int id, StaffUpsert item, CancellationToken cancellationToken)
    {
        var staff = await _repository.GetByIdAsync(id, cancellationToken);
        staff.Update(item.Name);
    }
}
