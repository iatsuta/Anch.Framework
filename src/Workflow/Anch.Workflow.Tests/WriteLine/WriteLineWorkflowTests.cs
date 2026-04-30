using System.Text;

using Anch.Workflow.Engine;
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

    [Fact]
    public async Task WriteText_TextReceived()
    {
        // Arrange

        // Act
        var wi = await this.StartWorkflow(new object());

        await this.writer.FlushAsync();

        // Assert
        wi.Status.Should().Be(WorkflowStatus.Finished);

        this.stringBuilder.ToString().Should().StartWith(WriteLineWorkflow.Message);

        (await this.Storage.GetWaitEvents()).Should().BeEmpty();
    }

    protected override IServiceCollection CreateServices()
    {
        return base.CreateServices()
            .AddSingleton<IDefaultOutput>(new DefaultOutput(this.writer));
    }
}