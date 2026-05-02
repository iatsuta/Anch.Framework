using Anch.Testing.Xunit;
using Anch.Workflow.DependencyInjection;
using Anch.Workflow.Domain.Runtime;
using Anch.Workflow.Tests._Base;
using Anch.Workflow.Tests.ParallelForeachApprove;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.Workflow.Tests.StartWorkflowsWithForksApprove;

public class StartWorkflowsWithForksApproveWorkflowTests : SingleScopeWorkflowTestBase<StartWorkflowsWithForksApproveWorkflowObject, StartWorkflowsWithForksApproveWorkflow>
{
    [AnchFact]
    public async Task SomeOneRejected_ApprovingItemsTerminated(CancellationToken ct)
    {
        // Arrange
        var wfObj = new StartWorkflowsWithForksApproveWorkflowObject
        {
            Items = Enumerable.Range(0, 5).Select(value => new StartWorkflowsWithForksApproveItemWorkflowObject { Name = "Item" + value }).ToList()
        };

        // Act
        await this.StartWorkflow(wfObj, ct);

        var startStatus = wfObj.Status;

        var waitApproveEvents = (await this.Storage.GetWaitEvents(ct)).Where(ei => ei.Header == StartWorkflowsWithForksApproveItemWorkflow.ApproveWaitEvent).ToList();

        var waitRejectEvents = (await this.Storage.GetWaitEvents(ct)).Where(ei => ei.Header == StartWorkflowsWithForksApproveItemWorkflow.RejectWaitEvent).ToList();

        var approvingInstances = waitApproveEvents.Select(e => e.TargetState.Workflow.Owner!.Workflow).ToArray();

        var exampleApproveEvent = waitApproveEvents.First();

        await this.Host.CreateExecutor(WorkflowExecutionPolicy.Full).PushEvent(exampleApproveEvent.Header, exampleApproveEvent.TargetState, cancellationToken: ct);

        var exampleRejectEvent = waitRejectEvents.First(e => e.TargetState.Workflow.Source != exampleApproveEvent.TargetState.Workflow.Source);

        await this.Host.CreateExecutor(WorkflowExecutionPolicy.Full).PushEvent(exampleRejectEvent.Header, exampleRejectEvent.TargetState, cancellationToken: ct);

        // Assert

        var usedWfObjects = new[] { exampleApproveEvent.TargetState.Workflow.Source, exampleRejectEvent.TargetState.Workflow.Source };

        var approvedWf = approvingInstances.Single(wf => wf.Source == exampleApproveEvent.TargetState.Workflow.Source);
        var rejectedWf = approvingInstances.Single(wf => wf.Source == exampleRejectEvent.TargetState.Workflow.Source);

        var notUsedApprovingInstances = approvingInstances.Where(wf => !usedWfObjects.Contains(wf.Source)).ToArray();

        Assert.Equal(StartWorkflowsWithForksApproveStatus.Approving, startStatus);

        Assert.Equal(WorkflowStatus.Finished, approvedWf.Status);
        Assert.Equal(WorkflowStatus.Finished, rejectedWf.Status);

        Assert.Equal(3, notUsedApprovingInstances.Length);
        Assert.Equal(WorkflowStatus.Terminated, notUsedApprovingInstances[0].Status);
        Assert.Equal(WorkflowStatus.Terminated, notUsedApprovingInstances[1].Status);
        Assert.Equal(WorkflowStatus.Terminated, notUsedApprovingInstances[2].Status);

        Assert.Equal(StartWorkflowsWithForksApproveStatus.Rejected, wfObj.Status);

        Assert.Empty(await this.Storage.GetWaitEvents(ct));
    }

    [AnchFact]
    public async Task AllApproved_RootWfApproved(CancellationToken ct)
    {
        // Arrange
        var wfObj = new StartWorkflowsWithForksApproveWorkflowObject
        {
            Items = Enumerable.Range(0, 5).Select(value => new StartWorkflowsWithForksApproveItemWorkflowObject { Name = "Item" + value }).ToList()
        };

        // Act
        await this.StartWorkflow(wfObj, ct);

        var startStatus = wfObj.Status;

        var waitApproveEvents = (await this.Storage.GetWaitEvents(ct)).Where(ei => ei.Header == StartWorkflowsWithForksApproveItemWorkflow.ApproveWaitEvent).ToList();

        var approvingInstances = waitApproveEvents.Select(e => e.TargetState.Workflow.Owner!.Workflow).ToArray();

        foreach (var waitApproveEvent in waitApproveEvents)
        {
            await this.Host.CreateExecutor(WorkflowExecutionPolicy.Full).PushEvent(waitApproveEvent.Header, waitApproveEvent.TargetState, cancellationToken: ct);
        }

        // Assert

        Assert.Equal(StartWorkflowsWithForksApproveStatus.Approving, startStatus);

        foreach (var approvingInstance in approvingInstances)
        {
            Assert.Equal(WorkflowStatus.Finished, approvingInstance.Status);
        }

        Assert.Equal(StartWorkflowsWithForksApproveStatus.Approved, wfObj.Status);

        Assert.Empty(await this.Storage.GetWaitEvents(ct));
    }


    [AnchFact]
    public async Task TerminateChildren_RootWf_Rejected(CancellationToken ct)
    {
        // Arrange
        var wfObj = new StartWorkflowsWithForksApproveWorkflowObject
        {
            Items = Enumerable.Range(0, 5).Select(value => new StartWorkflowsWithForksApproveItemWorkflowObject { Name = "Item" + value }).ToList()
        };

        // Act
        await this.StartWorkflow(wfObj, ct);

        var startStatus = wfObj.Status;

        var waitApproveEvents = (await this.Storage.GetWaitEvents(ct)).Where(ei => ei.Header == StartWorkflowsWithForksApproveItemWorkflow.ApproveWaitEvent).ToList();

        var approvingInstances = waitApproveEvents.Select(e => e.TargetState.Workflow.Owner!.Workflow).ToArray();

        await this.WorkflowMachineFactory.Create(approvingInstances[0]).Terminate(ct);

        // Assert

        Assert.Equal(StartWorkflowsWithForksApproveStatus.Approving, startStatus);

        foreach (var approvingInstance in approvingInstances)
        {
            Assert.Equal(WorkflowStatus.Terminated, approvingInstance.Status);
        }

        Assert.Equal(StartWorkflowsWithForksApproveStatus.Rejected, wfObj.Status);

        Assert.Empty(await this.Storage.GetWaitEvents(ct));
    }

    protected override void SetupWorkflow(IWorkflowSetup workflowSetup)
    {
        base.SetupWorkflow(workflowSetup);

        workflowSetup.Add<StartWorkflowsWithForksApproveItemWorkflow>();
    }
}