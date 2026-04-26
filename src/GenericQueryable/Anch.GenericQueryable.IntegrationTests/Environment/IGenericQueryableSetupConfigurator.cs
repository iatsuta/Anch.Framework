using Anch.GenericQueryable.DependencyInjection;

namespace Anch.GenericQueryable.IntegrationTests.Environment;

public interface IGenericQueryableSetupConfigurator
{
    void Configure(IGenericQueryableSetup builder);
}