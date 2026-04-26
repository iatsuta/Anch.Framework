using System.Collections.Concurrent;

using Anch.Core;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.Testing;

public class RootSharedServiceSource
{
    private readonly ConcurrentDictionary<(Type, Tuple<object?>?), object> cache = [];

    public TService GetSharedService<TService>(IServiceProvider serviceProvider, Tuple<object?>? key)
        where TService : notnull =>
        this.cache.GetOrAddAs((typeof(TService), key),
            _ => key == null
                ? serviceProvider.GetRequiredService<TService>()
                : serviceProvider.GetRequiredKeyedService<TService>(key.Item1));
}