namespace Anch.DependencyInjection.ServiceProxy;

/// <summary>One configured redirect of a type.</summary>
/// <param name="SourceType">The redirected type.</param>
/// <param name="TargetType">The implementation type, or the binder type when <see cref="IsBinder"/> is <c>true</c>.</param>
public record ServiceProxyTypeRedirectInfo(Type SourceType, Type TargetType)
{
    /// <summary>Whether this redirect overrides the previous ones.</summary>
    public required bool Replace { get; init; }

    /// <summary>Whether <c>TargetType</c> is an <c>IServiceProxyBinder</c> that computes the real target.</summary>
    public required bool IsBinder { get; init; }
}