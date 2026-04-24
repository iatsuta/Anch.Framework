namespace CommonFramework.Testing;

public interface ITestEnvironmentHook
{
    ValueTask Process(CancellationToken ct);
}