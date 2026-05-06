using Anch.Testing.Xunit;
using Anch.Workflow.Domain;
using Anch.Workflow.Domain.Runtime;
using Anch.Workflow.Tests._Base;

namespace Anch.Workflow.Tests.Wait;

public class WaitWorkflowTests : SingleScopeWorkflowTestBase<WaitWorkflowSource, WaitWorkflow>
{
    [AnchFact]
    public async ValueTask SendWaitedEvent_TestPassed(CancellationToken ct)
    {
        // Arrange

        // Act
        var wi = await this.StartWorkflow(new WaitWorkflowSource(), ct);

        var preWiStatus = wi.Status;

        var pushResult = await this.TillTheEndWorkflowExecutor
            .PushEvent(new EventHeader(WaitWorkflow.WaitEventName), wi.CurrentState, WaitWorkflow.WaitEventData, ct);

        // Assert
        Assert.Equal(WorkflowStatus.WaitEvent, preWiStatus);
        Assert.Contains(wi, pushResult.Modified);
        Assert.Equal(WorkflowStatus.Finished, wi.Status);

        Assert.Empty(await this.RootRepository.GetWaitEvents().ToListAsync(ct));
    }
}