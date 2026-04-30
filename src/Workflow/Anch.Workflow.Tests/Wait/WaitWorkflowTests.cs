using Anch.Workflow.Engine;
using Anch.Workflow.Tests._Base;

namespace Anch.Workflow.Tests.Wait;

public class WaitWorkflowTests : SingleScopeWorkflowTestBase<WaitWorkflowSource, WaitWorkflow>
{
    [Fact]
    public async Task SendWaitedEvent_TestPassed()
    {
        // Arrange

        // Act
        var wi = await this.StartWorkflow(new WaitWorkflowSource());

        var preWiStatus = wi.Status;

        var processedWorkflowInstances = await this.Host.PushEvent(new EventHeader(WaitWorkflow.WaitEventName), wi.CurrentState, WaitWorkflow.WaitEventData);

        // Assert
        preWiStatus.Should().Be(WorkflowStatus.WaitEvent);
        processedWorkflowInstances.Should().Contain(wi);
        wi.Status.Should().Be(WorkflowStatus.Finished);

        (await this.Storage.GetWaitEvents()).Should().BeEmpty();
    }
}