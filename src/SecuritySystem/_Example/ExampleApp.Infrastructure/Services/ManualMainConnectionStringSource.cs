namespace ExampleApp.Infrastructure.Services;

public class ManualMainConnectionStringSource(string connectionString) : IMainConnectionStringSource
{
    public string ConnectionString { get; } = connectionString;
}