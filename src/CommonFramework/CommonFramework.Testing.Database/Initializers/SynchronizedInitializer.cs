using CommonFramework.Threading;

namespace CommonFramework.Testing.Database.Initializers;

public class SynchronizedInitializer<T> : ISynchronizedInitializer<T>
{
    private readonly IAsyncLocker asyncLocker = new AsyncLocker();

    private bool initialized = false;

    public async Task Run(Func<Task> action)
    {
        if (!this.initialized)
        {
            using (await this.asyncLocker.CreateScope())
            {
                if (!this.initialized)
                {
                    await action();
                }
            }

            this.initialized = true;
        }
    }
}