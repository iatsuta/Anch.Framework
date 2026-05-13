namespace Anch.Testing.Database.ConnectionStringManagement;

public interface IActualTestConnectionStringFactory
{
    TestConnectionString Create(ServiceProviderIndex serviceProviderIndex);
}