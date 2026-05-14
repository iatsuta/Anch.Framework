namespace Anch.Testing.Database.ConnectionStringManagement;

public class ActualTestConnectionStringFactory(ITestConnectionStringFactory testConnectionStringFactory) : IActualTestConnectionStringFactory
{
    public TestConnectionString Create(ServiceProviderIndex serviceProviderIndex) =>
        testConnectionStringFactory.Create(serviceProviderIndex.IsMain ? "" : $"_pool_{serviceProviderIndex.Index:D5}");
}