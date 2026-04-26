namespace Anch.Testing.XunitEngine;

public interface IServiceProviderPool
{
    IServiceProvider Get();

    void Release(IServiceProvider serviceProvider);
}