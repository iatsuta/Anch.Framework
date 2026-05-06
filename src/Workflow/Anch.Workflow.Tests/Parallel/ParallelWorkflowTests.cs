using Anch.Testing.Xunit;
using Anch.Workflow.Domain.Runtime;
using Anch.Workflow.Tests._Base;

namespace Anch.Workflow.Tests.Parallel;

public class ParallelWorkflowTests : SingleScopeWorkflowTestBase<ParallelWorkflowObject, ParallelWorkflow>
{
    [AnchFact]
    public async ValueTask Parallel_SendApprove_RejectBranchTerminated(CancellationToken ct)
    {
        // Arrange
        var wfObj = new ParallelWorkflowObject();

        // Act
        var wi = await this.StartWorkflow(wfObj, ct);

        var preWfObjStatus = wfObj.Status;

        var waitEvents = await this.RootRepository.GetWaitEvents().ToListAsync(ct);

        var approveEvent = waitEvents.Single(ei => ei.Header == ParallelWorkflow.ApproveWaitEvent);

        var rejectEvent = waitEvents.Single(ei => ei.Header == ParallelWorkflow.RejectWaitEvent);

        await this.TillTheEndWorkflowExecutor.PushEvent(approveEvent.Header, approveEvent.TargetState, cancellationToken: ct);

        // Assert
        Assert.Equal(ParallelApproveStatus.Approving, preWfObjStatus);
        Assert.Equal(ParallelApproveStatus.Approved, wfObj.Status);

        Assert.Equal(WorkflowStatus.Finished, approveEvent.TargetState.Workflow.Status);
        Assert.Equal(WorkflowStatus.Terminated, rejectEvent.TargetState.Workflow.Status);

        Assert.Equal(WorkflowStatus.Finished, wi.Status);

        var finalEvents = await this.RootRepository.GetWaitEvents().ToListAsync(ct);

        Assert.Empty(finalEvents);
    }
}