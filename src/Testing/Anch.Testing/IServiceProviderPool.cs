namespace Anch.Testing;

public interface IServiceProviderPool
{
    IServiceProvider Get();

    ValueTask<IServiceProvider> GetAsync(CancellationToken ct);

    void Release(IServiceProvider serviceProvider);
}