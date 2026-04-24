using System.Collections.Concurrent;

using Microsoft.Extensions.DependencyInjection;

namespace CommonFramework.Testing.XunitEngine;

public class RootSharedServiceSource
{
    private readonly ConcurrentDictionary<(Type, Tuple<object?>?), object> cache = [];

    public TService GetSharedService<TService>(IServiceProvider serviceProvider, Tuple<object?>? key)
        where TService : notnull =>
        (TService)this.cache.GetOrAdd((typeof(TService), key),
            _ => key == null
                ? serviceProvider.GetRequiredService<TService>()
                : serviceProvider.GetRequiredKeyedService<TService>(key.Item1));
}