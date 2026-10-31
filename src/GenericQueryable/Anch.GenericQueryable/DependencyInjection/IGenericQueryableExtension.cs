using Microsoft.Extensions.DependencyInjection;

namespace Anch.GenericQueryable.DependencyInjection;

/// <summary>An extension that registers additional services together with GenericQueryable.</summary>
public interface IGenericQueryableExtension
{
    /// <summary>Registers the services.</summary>
    /// <param name="services">The service collection.</param>
    public void AddServices(IServiceCollection services);
}