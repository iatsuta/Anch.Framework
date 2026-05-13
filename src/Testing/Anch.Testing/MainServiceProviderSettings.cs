namespace Anch.Testing;

public record MainServiceProviderSettings(bool AllowParallelization, bool ReturnToServicePool)
{
    public static MainServiceProviderSettings Default { get; } = new (true, true);
}