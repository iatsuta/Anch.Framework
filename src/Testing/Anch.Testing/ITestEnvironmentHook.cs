namespace Anch.Testing;

public interface ITestEnvironmentHook
{
    ValueTask Process(CancellationToken ct);
}