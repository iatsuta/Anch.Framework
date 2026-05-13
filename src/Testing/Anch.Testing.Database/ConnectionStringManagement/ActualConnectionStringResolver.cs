namespace Anch.Testing.Database.ConnectionStringManagement;

public class ActualConnectionStringResolver(
    ITestConnectionStringProvider testConnectionStringProvider,
    ITestConnectionStringBuilder testConnectionStringBuilder) : IActualConnectionStringResolver
{
    public TestConnectionString GetActualConnectionString(ServiceProviderIndex serviceProviderIndex) =>

        serviceProviderIndex.IsMain ? testConnectionStringProvider.Main : testConnectionStringBuilder.AddPostfix($"_pool_{serviceProviderIndex.Index:D5}");
}