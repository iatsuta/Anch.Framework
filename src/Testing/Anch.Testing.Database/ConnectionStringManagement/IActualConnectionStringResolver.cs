namespace Anch.Testing.Database.ConnectionStringManagement;

public interface IActualConnectionStringResolver
{
    TestConnectionString GetActualConnectionString(ServiceProviderIndex serviceProviderIndex);
}