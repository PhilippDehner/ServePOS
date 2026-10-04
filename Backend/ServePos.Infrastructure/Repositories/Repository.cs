namespace ServePos.Infrastructure.Repositories;

public abstract class Repository(PosDbContext context)
{
    protected PosDbContext Context { get; } = context;

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await Context.SaveChangesAsync(cancellationToken);
    }
}
