using System.Text;

using Anch.Testing.Xunit;
using Anch.Workflow.Domain.Runtime;
using Anch.Workflow.States.Output;
using Anch.Workflow.Tests._Base;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.Workflow.Tests.WriteLine;

public class WriteLineWorkflowTests : SingleScopeWorkflowTestBase<object, WriteLineWorkflow>
{
    private readonly StringBuilder stringBuilder = new();

    private readonly StringWriter writer;

    public WriteLineWorkflowTests()
    {
        this.writer = new StringWriter(this.stringBuilder);
    }

    [AnchFact]
    public async Task WriteText_TextReceived(CancellationToken ct)
    {
        // Arrange

        // Act
        var wi = await this.StartWorkflow(new object(), ct);

        await this.writer.FlushAsync();

        // Assert
        Assert.Equal(WorkflowStatus.Finished, wi.Status);

        Assert.StartsWith(WriteLineWorkflow.Message, this.stringBuilder.ToString());

        Assert.Empty(await this.Storage.GetWaitEvents(ct));
    }

    protected override IServiceCollection CreateServices(IServiceCollection services)
    {
        return base.CreateServices(services)
            .AddSingleton<IDefaultOutput>(new DefaultOutput(this.writer));
    }
}