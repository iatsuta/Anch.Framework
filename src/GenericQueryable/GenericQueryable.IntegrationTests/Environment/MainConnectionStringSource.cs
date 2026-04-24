namespace GenericQueryable.IntegrationTests.Environment;

public class MainConnectionStringSource(string connectionString) : IMainConnectionStringSource
{
    public string ConnectionString { get; } = connectionString;
}