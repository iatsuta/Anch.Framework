using Microsoft.Extensions.DependencyInjection;

namespace Anch.SecuritySystem.Configurator;

/// <summary>Collects the configurator modules.</summary>
public class ConfiguratorSetup : IConfiguratorSetup
{
    private readonly List<IConfiguratorModule> modules = [];

    public IConfiguratorSetup AddModule(IConfiguratorModule module)
    {
        this.modules.Add(module);

        return this;
    }

    public void Initialize(IServiceCollection services)
    {
        this.modules.ForEach(module =>
                             {
                                 services.AddSingleton(module);
                                 module.AddServices(services);
                             });
    }
}
