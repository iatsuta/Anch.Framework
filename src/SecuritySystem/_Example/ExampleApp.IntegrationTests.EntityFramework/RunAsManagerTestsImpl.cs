using ExampleApp.IntegrationTests.Environment;

namespace ExampleApp.IntegrationTests;

public class RunAsManagerTestsImpl(IServiceProvider rootServiceProvider) : RunAsManagerTests(rootServiceProvider);