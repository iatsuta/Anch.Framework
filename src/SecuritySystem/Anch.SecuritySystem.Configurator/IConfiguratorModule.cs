using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Anch.SecuritySystem.Configurator;

/// <summary>A module that appears in the configurator and can add its own pages and handlers.</summary>
public interface IConfiguratorModule
{
    string Name { get; }

    void AddServices(IServiceCollection services);

    void MapApi(IEndpointRouteBuilder endpointsBuilder, string route);
}
