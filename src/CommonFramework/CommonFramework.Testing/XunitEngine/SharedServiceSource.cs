namespace CommonFramework.Testing.XunitEngine;

public class SharedServiceSource(RootSharedServiceSource rootSharedServiceSource, IServiceProvider serviceProvider) : ISharedServiceSource
{
    public TService GetSharedService<TService>()
        where TService : notnull =>
        rootSharedServiceSource.GetSharedService<TService>(serviceProvider, null);

    public TService GetSharedService<TService>(object? key)
        where TService : notnull
    {
        return rootSharedServiceSource.GetSharedService<TService>(serviceProvider, Tuple.Create(key));
    }
}