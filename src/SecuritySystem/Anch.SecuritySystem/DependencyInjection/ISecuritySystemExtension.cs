using Microsoft.Extensions.DependencyInjection;

namespace Anch.SecuritySystem.DependencyInjection;

public interface ISecuritySystemExtension
{
    public void AddServices(IServiceCollection services);
}
