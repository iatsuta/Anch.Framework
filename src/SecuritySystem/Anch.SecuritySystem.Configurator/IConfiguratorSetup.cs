namespace Anch.SecuritySystem.Configurator;

/// <summary>Fluent configuration of the configurator.</summary>
public interface IConfiguratorSetup
{
    IConfiguratorSetup AddModule(IConfiguratorModule module);
}
