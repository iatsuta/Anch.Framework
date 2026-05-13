namespace Anch.Testing;

public record ServiceProviderIndex(int Index)
{
    public bool IsMain => this == Main;

    public static ServiceProviderIndex Main { get; } = new(-1);
}