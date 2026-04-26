using Microsoft.Extensions.DependencyInjection;

namespace Anch.SecuritySystem.DependencyInjection;

public class SecuritySystemExtension(Action<IServiceCollection> addServicesAction) : ISecuritySystemExtension
{
    public void AddServices(IServiceCollection services) => addServicesAction(services);
}
