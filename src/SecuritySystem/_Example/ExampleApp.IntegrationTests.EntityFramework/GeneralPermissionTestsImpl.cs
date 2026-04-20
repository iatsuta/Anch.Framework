using ExampleApp.IntegrationTests.Environment;

namespace ExampleApp.IntegrationTests;

public class GeneralPermissionTestsImpl(IServiceProvider rootServiceProvider) : GeneralPermissionTests(rootServiceProvider);