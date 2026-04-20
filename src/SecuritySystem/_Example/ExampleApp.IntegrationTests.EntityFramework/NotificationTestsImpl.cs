using ExampleApp.IntegrationTests.Environment;

namespace ExampleApp.IntegrationTests;

public class NotificationTestsImpl(IServiceProvider rootServiceProvider) : NotificationTests(rootServiceProvider);