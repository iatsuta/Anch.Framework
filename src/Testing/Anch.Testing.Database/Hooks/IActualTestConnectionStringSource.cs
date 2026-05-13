using Anch.Testing.Database.ConnectionStringManagement;

namespace Anch.Testing.Database.Hooks;

public interface IActualTestConnectionStringSource
{
    TestConnectionString ActualConnectionString { get; }
}