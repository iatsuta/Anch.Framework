using Anch.Testing.Xunit;
using Anch.Workflow.DependencyInjection;
using Anch.Workflow.Domain.Runtime;
using Anch.Workflow.Tests._Base;

namespace Anch.Workflow.Tests.StartWorkflowsWithTaskApprove;

public class StartWorkflowsWithTaskApproveWorkflowTests : SingleScopeWorkflowTestBase<StartWorkflowsWithTaskApproveWorkflowObject,
    StartWorkflowsWithTaskApproveWorkflow>
{
    [AnchFact]
    public async ValueTask SomeOneRejected_ApprovingItemsTerminated(CancellationToken ct)
    {
        // Arrange
        var wfObj = new StartWorkflowsWithTaskApproveWorkflowObject
        {
            Items = [.. Enumerable.Range(0, 5).Select(value => new StartWorkflowsWithTaskApproveItemWorkflowObject { Name = "Item" + value })]
        };

        // Act
        await this.StartWorkflow(wfObj, ct);

        var startStatus = wfObj.Status;

        var waitApproveEvents = await this.RootRepository.GetWaitEvents().Where(ei => ei.Header == StartWorkflowsWithTaskApproveItemWorkflow.ApproveWaitEvent).ToListAsync(ct);

        var waitRejectEvents = await this.RootRepository.GetWaitEvents().Where(ei => ei.Header == StartWorkflowsWithTaskApproveItemWorkflow.RejectWaitEvent).ToListAsync(ct);

        var approvingInstances = waitApproveEvents.Select(e => e.TargetState.Workflow).ToArray();

        var exampleApproveEvent = waitApproveEvents.First();

        await this.TillTheEndWorkflowExecutor
            .PushEvent(exampleApproveEvent.Header, exampleApproveEvent.TargetState, cancellationToken: ct);

        var exampleRejectEvent = waitRejectEvents.First(e => e.TargetState.Workflow.Source != exampleApproveEvent.TargetState.Workflow.Source);

        await this.TillTheEndWorkflowExecutor
            .PushEvent(exampleRejectEvent.Header, exampleRejectEvent.TargetState, cancellationToken: ct);

        // Assert

        var usedWfObjects = new[] { exampleApproveEvent.TargetState.Workflow.Source, exampleRejectEvent.TargetState.Workflow.Source };

        var approvedWf = approvingInstances.Single(wf => wf.Source == exampleApproveEvent.TargetState.Workflow.Source);
        var rejectedWf = approvingInstances.Single(wf => wf.Source == exampleRejectEvent.TargetState.Workflow.Source);

        var notUsedApprovingInstances = approvingInstances.Where(wf => !usedWfObjects.Contains(wf.Source)).ToArray();

        Assert.Equal(StartWorkflowsWithTaskApproveStatus.Approving, startStatus);

        Assert.Equal(WorkflowStatus.Finished, approvedWf.Status);
        Assert.Equal(WorkflowStatus.Finished, rejectedWf.Status);

        Assert.Equal(3, notUsedApprovingInstances.Length);
        Assert.Equal(WorkflowStatus.Terminated, notUsedApprovingInstances[0].Status);
        Assert.Equal(WorkflowStatus.Terminated, notUsedApprovingInstances[1].Status);
        Assert.Equal(WorkflowStatus.Terminated, notUsedApprovingInstances[2].Status);

        Assert.Equal(StartWorkflowsWithTaskApproveStatus.Rejected, wfObj.Status);

        var finalEvents = await this.RootRepository.GetWaitEvents().ToListAsync(ct);

        Assert.Empty(finalEvents);
    }

    [AnchFact]
    public async ValueTask AllApproved_RootWfApproved(CancellationToken ct)
    {
        // Arrange
        var wfObj = new StartWorkflowsWithTaskApproveWorkflowObject
        {
            Items = [.. Enumerable.Range(0, 5).Select(value => new StartWorkflowsWithTaskApproveItemWorkflowObject { Name = "Item" + value })]
        };

        // Act
        await this.StartWorkflow(wfObj, ct);

        var startStatus = wfObj.Status;

        var waitApproveEvents = await this.RootRepository.GetWaitEvents().Where(ei => ei.Header == StartWorkflowsWithTaskApproveItemWorkflow.ApproveWaitEvent).ToListAsync(ct);

        var approvingInstances = waitApproveEvents.Select(e => e.TargetState.Workflow).ToArray();

        foreach (var waitApproveEvent in waitApproveEvents)
        {
            await this.TillTheEndWorkflowExecutor
                .PushEvent(waitApproveEvent.Header, waitApproveEvent.TargetState, cancellationToken: ct);
        }

        // Assert

        Assert.Equal(StartWorkflowsWithTaskApproveStatus.Approving, startStatus);

        foreach (var approvingInstance in approvingInstances)
        {
            Assert.Equal(WorkflowStatus.Finished, approvingInstance.Status);
        }

        Assert.Equal(StartWorkflowsWithTaskApproveStatus.Approved, wfObj.Status);

        Assert.Empty(await this.RootRepository.GetWaitEvents().ToListAsync(ct));
    }


    [AnchFact]
    public async ValueTask TerminateChildren_RootWf_Rejected(CancellationToken ct)
    {
        // Arrange
        var wfObj = new StartWorkflowsWithTaskApproveWorkflowObject
        {
            Items = [.. Enumerable.Range(0, 5).Select(value => new StartWorkflowsWithTaskApproveItemWorkflowObject { Name = "Item" + value })]
        };

        // Act
        await this.StartWorkflow(wfObj, ct);

        var startStatus = wfObj.Status;

        var waitApproveEvents = await this.RootRepository.GetWaitEvents().Where(ei => ei.Header == StartWorkflowsWithTaskApproveItemWorkflow.ApproveWaitEvent).ToListAsync(ct);

        var approvingInstances = waitApproveEvents.Select(e => e.TargetState.Workflow).ToArray();

        await this.TillTheEndWorkflowExecutor.Terminate(approvingInstances[0], ct);

        // Assert

        Assert.Equal(StartWorkflowsWithTaskApproveStatus.Approving, startStatus);

        foreach (var approvingInstance in approvingInstances)
        {
            Assert.Equal(WorkflowStatus.Terminated, approvingInstance.Status);
        }

        Assert.Equal(StartWorkflowsWithTaskApproveStatus.Rejected, wfObj.Status);

        Assert.Empty(await this.RootRepository.GetWaitEvents().ToListAsync(ct));
    }

    protected override void SetupWorkflow(IWorkflowSetup workflowSetup) =>

        base.SetupWorkflow(workflowSetup.Add<StartWorkflowsWithTaskApproveItemWorkflow>());
}