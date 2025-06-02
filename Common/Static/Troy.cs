namespace Common.Static;

public static class Troy
{
    public static async Task Try(Func<Task> exec, Func<Exception, Task> error = null, Action fin = null)
    {
        await Try(
            async () =>
            {
                await exec();
                return 0;
            },
            async ex =>
            {
                if (error is not null)
                    await error(ex);
                return 1;
            },
            fin);
    }

    public static async Task<T> Try<T>(Func<Task<T>> exec, Func<Exception, Task<T>> error = null, Action fin = null)
    {
        try
        {
            return await exec();
        }
        catch (Exception ex)
        {
            return error is not null
                ? await error(ex)
                : default;
        }
        finally
        {
            fin?.Invoke();
        }
    }
}
