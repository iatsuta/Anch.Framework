namespace Anch.Testing.Database.ConnectionStringManagement;

public class ActualTestConnectionStringFactory(
    ITestConnectionStringProvider testConnectionStringProvider,
    ITestConnectionStringBuilder testConnectionStringBuilder) : IActualTestConnectionStringFactory
{
    public TestConnectionString Create(ServiceProviderIndex serviceProviderIndex) =>
        serviceProviderIndex.IsMain ? testConnectionStringProvider.Main : testConnectionStringBuilder.AddPostfix($"_pool_{serviceProviderIndex.Index:D5}");
}