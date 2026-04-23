using System.Collections.Concurrent;

namespace CommonFramework.Threading;

public class AsyncLockerProvider : IAsyncLockerProvider
{
    private readonly ConcurrentDictionary<object, IAsyncLocker> cache = new();

    public IAsyncLocker CreateLocker(object key) => this.cache.GetOrAdd(key, _ => new AsyncLocker());
}