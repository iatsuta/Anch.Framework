using ExampleApp.IntegrationTests.Environment;

namespace ExampleApp.IntegrationTests;

public class ImpersonateServiceTestsImpl(IServiceProvider rootServiceProvider) : ImpersonateServiceTests(rootServiceProvider);