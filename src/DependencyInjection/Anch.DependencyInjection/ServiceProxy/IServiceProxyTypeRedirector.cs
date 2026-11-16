namespace Anch.DependencyInjection.ServiceProxy;

/// <summary>Finds the implementation type a requested type is redirected to.</summary>
public interface IServiceProxyTypeRedirector
{
    /// <summary>Returns the redirect target of a type.</summary>
    /// <param name="sourceType">The requested type.</param>
    /// <returns>The target type, or <c>null</c> if the type is not redirected.</returns>
    Type? TryRedirect(Type sourceType);
}