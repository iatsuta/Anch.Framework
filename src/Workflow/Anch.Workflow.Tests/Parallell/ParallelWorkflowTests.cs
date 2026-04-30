using Anch.Workflow.Engine;
using Anch.Workflow.Tests._Base;

namespace Anch.Workflow.Tests.Parallell;

public class ParallelWorkflowTests : SingleScopeWorkflowTestBase<ParallelWorkflowObject, ParallelWorkflow>
{
    [Fact]
    public async System.Threading.Tasks.Task Parallel_SendApprove_RejectBranchTerminated()
    {
        // Arrange
        var wfObj = new ParallelWorkflowObject();

        // Act
        var wi = await this.StartWorkflow(wfObj);

        var preWfObjStatus = wfObj.Status;

        var waitEvents = (await this.Storage.GetWaitEvents());

        var approveEvent = waitEvents.Single(ei => ei.Event == ParallelWorkflow.ApproveWaitEvent);

        var rejectEvent = waitEvents.Single(ei => ei.Event == ParallelWorkflow.RejectWaitEvent);

        await this.Host.PushEvent(approveEvent.Event, approveEvent.TargetState);

        // Assert
        preWfObjStatus.Should().Be(ParallelApproveStatus.Approving);
        wfObj.Status.Should().Be(ParallelApproveStatus.Approved);

        approveEvent.TargetState.Workflow.Status.Should().Be(WorkflowStatus.Finished);
        rejectEvent.TargetState.Workflow.Status.Should().Be(WorkflowStatus.Terminated);

        wi.Status.Should().Be(WorkflowStatus.Finished);

        (await this.Storage.GetWaitEvents()).Should().BeEmpty();
    }
}