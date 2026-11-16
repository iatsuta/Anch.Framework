using Microsoft.Extensions.DependencyInjection;

namespace Anch.SecuritySystem.DependencyInjection;

/// <summary>An extension that registers additional services together with the security system.</summary>
public interface ISecuritySystemExtension
{
    /// <summary>Registers the services.</summary>
    /// <param name="services">The service collection.</param>
    public void AddServices(IServiceCollection services);
}
