namespace Anch.DependencyInjection.ServiceProxy;

/// <summary>Configures redirection of service types to other implementation types.</summary>
public interface IServiceProxySetup
{
    /// <summary>Redirects a type to a fixed implementation type.</summary>
    /// <param name="sourceType">The type to redirect; may be an open generic type.</param>
    /// <param name="targetType">The implementation type; may be an open generic type.</param>
    /// <param name="replace">When <c>false</c>, this must be the first redirect of the type; when <c>true</c>, it overrides the previous ones.</param>
    IServiceProxySetup SetRedirect(Type sourceType, Type targetType, bool replace) => this.SetRedirect(sourceType, _ => targetType, replace);

    /// <summary>Redirects a type to an implementation type chosen at run time.</summary>
    /// <param name="sourceType">The type to redirect; may be an open generic type.</param>
    /// <param name="targetTypeSelector">Chooses the implementation type.</param>
    /// <param name="replace">When <c>false</c>, this must be the first redirect of the type; when <c>true</c>, it overrides the previous ones.</param>
    IServiceProxySetup SetRedirect(Type sourceType, Func<IServiceProvider, Type> targetTypeSelector, bool replace);

    /// <summary>Redirects a type to the implementation type that a binder computes.</summary>
    /// <param name="sourceType">The type to redirect; may be an open generic type.</param>
    /// <param name="binderType">An <c>IServiceProxyBinder</c> that returns the target type.</param>
    /// <param name="replace">When <c>false</c>, this must be the first redirect of the type; when <c>true</c>, it overrides the previous ones.</param>
    IServiceProxySetup BindRedirect(Type sourceType, Type binderType, bool replace);
}