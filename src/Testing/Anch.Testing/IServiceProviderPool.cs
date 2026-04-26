namespace Anch.Testing;

public interface IServiceProviderPool
{
    IServiceProvider Get();

    void Release(IServiceProvider serviceProvider);
}