using Xunit;

namespace Anch.Testing.Xunit.Engine;

/// <summary>Extensions for async lifetime handling of test objects.</summary>
public static class AsyncLifetimeExtensions
{
    public static async ValueTask<IAsyncDisposable?> TryCreateScopeAsync(this IAsyncLifetime? asyncLifetime, CancellationToken ct)
    {
        if (asyncLifetime is null)
        {
            return null;
        }
        else
        {
            await asyncLifetime.InitializeAsync();

            return new AsyncLifetimeScope(asyncLifetime);
        }
    }

    private class AsyncLifetimeScope(IAsyncLifetime asyncLifetime) : IAsyncDisposable
    {
        public ValueTask DisposeAsync() => asyncLifetime.DisposeAsync();
    }
}