namespace ExampleApp.Infrastructure.Services;

public interface IMainConnectionStringSource
{
    string ConnectionString { get; }
}