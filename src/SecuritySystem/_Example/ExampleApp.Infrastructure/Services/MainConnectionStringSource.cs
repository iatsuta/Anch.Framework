namespace ExampleApp.Infrastructure.Services;

public class MainConnectionStringSource(string connectionString) : IMainConnectionStringSource
{
    public string ConnectionString { get; } = connectionString;
}