using DAL.EF;

namespace BLL.Base;

public abstract class ContextHasService : IDisposable
{
    protected AppDbContext Context { get; }

    protected ContextHasService(AppDbContext context)
    {
        Context = context;
    }

    protected async Task SaveChanges()
    {
        await Context.SaveChangesAsync();
    }

    protected async Task ExecTran<T>(Func<T, Task> exec, T arg)
    {
        await using var db = await Context.Database.BeginTransactionAsync();
        try
        {
            await exec(arg);
            await db.CommitAsync();
        }
        catch
        {
            await db.RollbackAsync();
            throw;
        }
    }

    public void Dispose()
    {
        Context?.Dispose();
    }
}
