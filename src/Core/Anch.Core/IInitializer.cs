namespace Anch.Core;

/// <summary>A service that performs one-time asynchronous initialization (for example filling a database or rebuilding denormalized data). Registered as a keyed service and run by the test infrastructure.</summary>
public interface IInitializer
{
    /// <summary>Runs the initialization.</summary>
    /// <param name="ct">Cancellation token.</param>
    Task Initialize(CancellationToken ct);
}