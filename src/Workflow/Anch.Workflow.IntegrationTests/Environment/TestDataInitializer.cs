using Anch.Core;

namespace Anch.Workflow.IntegrationTests.Environment;

public class TestDataInitializer : IInitializer
{
    public Task Initialize(CancellationToken cancellationToken) => Task.CompletedTask;
}