namespace Anch.GenericQueryable.IntegrationTests.Environment;

public interface IMainConnectionStringSource
{
    string ConnectionString { get; }
}