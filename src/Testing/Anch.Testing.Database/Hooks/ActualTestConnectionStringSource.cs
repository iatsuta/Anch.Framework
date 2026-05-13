using Anch.Testing.Database.ConnectionStringManagement;

namespace Anch.Testing.Database.Hooks;

public record ActualTestConnectionStringSource(TestConnectionString ActualConnectionString) : IActualTestConnectionStringSource;