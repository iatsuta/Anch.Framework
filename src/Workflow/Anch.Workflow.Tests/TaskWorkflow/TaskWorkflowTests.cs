using Anch.Testing.Xunit;
using Anch.Workflow.Domain.Runtime;
using Anch.Workflow.Tests._Base;

namespace Anch.Workflow.Tests.TaskWorkflow;

public class TaskWorkflowTests : SingleScopeWorkflowTestBase<TaskWorkflowObject, TaskWorkflow>
{
    [AnchFact]
    public async ValueTask Task_SendApproveCommand_WorkflowApproved(CancellationToken ct)
    {
        // Arrange
        var wfObj = new TaskWorkflowObject();

        // Act
        var wi = await this.StartWorkflow(wfObj, ct);

        var preWfObjStatus = wfObj.Status;

        var approveEvent = await this.RootRepository.GetWaitEvents().Where(ei => ei.Header == TaskWorkflow.ApproveEventHeader).SingleAsync(ct);

        await this.TillTheEndWorkflowExecutor.PushEvent(approveEvent.Header, approveEvent.TargetState, cancellationToken: ct);

        // Assert
        Assert.Equal(TaskApproveStatus.Approving, preWfObjStatus);
        Assert.Equal(TaskApproveStatus.Approved, wfObj.Status);

        Assert.True(wfObj.PostProcessWork);

        Assert.Equal(WorkflowStatus.Finished, wi.Status);

        Assert.Empty(await this.RootRepository.GetWaitEvents().ToListAsync(ct));
    }
}