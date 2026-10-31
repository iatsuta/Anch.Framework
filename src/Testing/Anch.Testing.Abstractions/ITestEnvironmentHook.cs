namespace Anch.Testing;

/// <summary>A step that runs before or after every test, for example restoring a database or clearing storage. Register with <c>AddEnvironmentHook</c>.</summary>
public interface ITestEnvironmentHook
{
    /// <summary>Runs the hook.</summary>
    /// <param name="ct">Cancellation token.</param>
    ValueTask Process(CancellationToken ct);
}