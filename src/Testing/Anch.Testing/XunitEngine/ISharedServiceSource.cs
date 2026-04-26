namespace Anch.Testing.XunitEngine;

public interface ISharedServiceSource
{
    TService GetSharedService<TService>()
        where TService : notnull;

    TService GetSharedService<TService>(object? key)
        where TService : notnull;
}