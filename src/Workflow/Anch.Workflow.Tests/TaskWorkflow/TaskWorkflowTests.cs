using Anch.Workflow.Engine;
using Anch.Workflow.Tests._Base;

namespace Anch.Workflow.Tests.TaskWorkflow;

public class TaskWorkflowTests : SingleScopeWorkflowTestBase<TaskWorkflowObject, TaskWorkflow>
{
    [Fact]
    public async System.Threading.Tasks.Task Task_SendApproveCommand_WorkflowApproved()
    {
        // Arrange
        var wfObj = new TaskWorkflowObject();

        // Act
        var wi = await this.StartWorkflow(wfObj);

        var preWfObjStatus = wfObj.Status;

        var approveEvent = (await this.Storage.GetWaitEvents()).Single(ei => ei.Event == TaskWorkflow.ApproveEventHeader);

        await this.Host.PushEvent(approveEvent.Event, approveEvent.TargetState);

        // Assert
        preWfObjStatus.Should().Be(TaskApproveStatus.Approving);
        wfObj.Status.Should().Be(TaskApproveStatus.Approved);

        wfObj.PostProcessWork.Should().BeTrue();

        wi.Status.Should().Be(WorkflowStatus.Finished);

        (await this.Storage.GetWaitEvents()).Should().BeEmpty();
    }
}