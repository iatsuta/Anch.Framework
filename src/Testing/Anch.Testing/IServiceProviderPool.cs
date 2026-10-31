namespace Anch.Testing;

/// <summary>A pool of service containers: each test takes a free one and returns it afterwards, so tests can run in parallel.</summary>
public interface IServiceProviderPool : IAsyncDisposable
{
    ValueTask<IServiceProvider> GetAsync(CancellationToken ct);

    ValueTask ReleaseAsync(IServiceProvider serviceProvider, CancellationToken ct);
}