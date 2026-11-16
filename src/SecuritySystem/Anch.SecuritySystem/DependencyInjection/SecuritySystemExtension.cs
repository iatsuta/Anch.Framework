using Microsoft.Extensions.DependencyInjection;

namespace Anch.SecuritySystem.DependencyInjection;

/// <summary>An extension that registers services with a delegate.</summary>
public class SecuritySystemExtension(Action<IServiceCollection> addServicesAction) : ISecuritySystemExtension
{
    public void AddServices(IServiceCollection services) => addServicesAction(services);
}
