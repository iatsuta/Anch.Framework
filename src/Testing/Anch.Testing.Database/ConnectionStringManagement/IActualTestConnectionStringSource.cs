namespace Anch.Testing.Database.ConnectionStringManagement;

public interface IActualTestConnectionStringSource
{
    TestConnectionString ActualConnectionString { get; }
}