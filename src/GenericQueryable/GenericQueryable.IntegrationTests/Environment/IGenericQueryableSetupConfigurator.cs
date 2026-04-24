using GenericQueryable.DependencyInjection;

namespace GenericQueryable.IntegrationTests.Environment;

public interface IGenericQueryableSetupConfigurator
{
    void Configure(IGenericQueryableSetup builder);
}