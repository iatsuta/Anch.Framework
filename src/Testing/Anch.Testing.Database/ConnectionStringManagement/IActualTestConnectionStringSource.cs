namespace Anch.Testing.Database.ConnectionStringManagement;

/// <summary>Supplies the connection string of the database that the current container uses.</summary>
public interface IActualTestConnectionStringSource
{
    TestConnectionString ActualConnectionString { get; }
}