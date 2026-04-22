using CommonFramework.Testing.XunitEngine;
using CommonFramework.Threading;

namespace CommonFramework.Testing.Database;

public class SynchronizedInitializer<T>(IServiceProviderSynchronizationContext serviceProviderSynchronizationContext) : ISynchronizedInitializer<T>
{
    private readonly IAsyncLocker asyncLocker = serviceProviderSynchronizationContext.AsyncLockerProvider.CreateLocker(typeof(T));

    private bool initialized = false;

    public async ValueTask Run(Func<Task> action)
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