using Anch.Testing.Database.ConnectionStringManagement;

namespace Anch.Testing.Database.Initializers;

public interface IActualConnectionStringResolver
{
    TestConnectionString GetActualConnectionString(ServiceProviderIndex serviceProviderIndex);
}

public class ActualConnectionStringResolver : IActualConnectionStringResolver
{
    public TestConnectionString GetActualConnectionString(ServiceProviderIndex serviceProviderIndex)
    {

        // testDatabaseConnectionStringBuilder.AddPostfix($"_pool_{serviceProviderIndex.Index:D5}");
        throw new NotImplementedException();
    }
}