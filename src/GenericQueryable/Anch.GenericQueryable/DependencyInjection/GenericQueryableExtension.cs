using Microsoft.Extensions.DependencyInjection;

namespace Anch.GenericQueryable.DependencyInjection;

/// <summary>An extension that registers additional services with a delegate.</summary>
/// <param name="setupAction">Registers the services.</param>
public class GenericQueryableExtension(Action<IServiceCollection> setupAction) : IGenericQueryableExtension
{
    /// <inheritdoc/>
    public void AddServices(IServiceCollection services) => setupAction(services);
}