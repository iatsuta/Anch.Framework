using ExampleApp.IntegrationTests.Environment;

namespace ExampleApp.IntegrationTests;

public class ConfiguratorApiTestsImpl(IServiceProvider rootServiceProvider) : ConfiguratorApiTests(rootServiceProvider);