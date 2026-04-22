namespace GenericQueryable.IntegrationTests.Environment;

public class MainConnectionStringSource : IMainConnectionStringSource
{
    public string ConnectionString { get; } = "Data Source=test.db";
}