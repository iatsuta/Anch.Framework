namespace Anch.DependencyInjection.ServiceProxy;

public record ServiceProxyTypeRedirectInfo(Type SourceType, Type TargetType)
{
    public required bool Replace { get; init; }

    public required bool IsBinder { get; init; }
}