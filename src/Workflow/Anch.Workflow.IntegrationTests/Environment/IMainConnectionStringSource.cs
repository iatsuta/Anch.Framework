namespace Anch.Workflow.IntegrationTests.Environment;

public interface IMainConnectionStringSource
{
    string ConnectionString { get; }
}