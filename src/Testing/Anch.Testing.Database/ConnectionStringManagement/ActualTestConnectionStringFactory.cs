namespace Anch.Testing.Database.ConnectionStringManagement;

public class ActualTestConnectionStringFactory(
    IActualTestConnectionStringSource actualTestConnectionStringSource,
    ITestConnectionStringBuilder testConnectionStringBuilder) : IActualTestConnectionStringFactory
{
    public TestConnectionString Create(ServiceProviderIndex serviceProviderIndex) =>
        serviceProviderIndex.IsMain ? actualTestConnectionStringSource.ActualConnectionString : testConnectionStringBuilder.AddPostfix($"_pool_{serviceProviderIndex.Index:D5}");
}