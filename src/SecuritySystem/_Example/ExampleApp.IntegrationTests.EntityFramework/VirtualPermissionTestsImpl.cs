using ExampleApp.IntegrationTests.Environment;

namespace ExampleApp.IntegrationTests;

public class VirtualPermissionTestsImpl(IServiceProvider rootServiceProvider) : VirtualPermissionTests(rootServiceProvider);