namespace CommonFramework.DependencyInjection.ServiceProxy;

public interface IServiceProxySetup
{
    IServiceProxySetup SetRedirect(Type sourceType, Type targetType, bool replace) => this.SetRedirect(sourceType, _ => targetType, replace);

    IServiceProxySetup SetRedirect(Type sourceType, Func<IServiceProvider, Type> targetTypeSelector, bool replace);

    IServiceProxySetup BindRedirect(Type sourceType, Type binderType, bool replace);
}