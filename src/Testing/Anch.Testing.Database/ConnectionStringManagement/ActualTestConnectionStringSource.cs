namespace Anch.Testing.Database.ConnectionStringManagement;

/// <summary>The connection string of the database that the current container uses.</summary>
public record ActualTestConnectionStringSource(TestConnectionString ActualConnectionString) : IActualTestConnectionStringSource;