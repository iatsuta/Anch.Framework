using Anch.Workflow.DependencyInjection;
using Anch.Workflow.Engine;
using Anch.Workflow.Tests._Base;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.Workflow.Tests.StartWorkflowsWithTaskApprove;

public class StartWorkflowsWithTaskApproveWorkflowTests : SingleScopeWorkflowTestBase<StartWorkflowsWithTaskApproveWorkflowObject, StartWorkflowsWithTaskApproveWorkflow>
{
    [Fact]
    public async Task SomeOneRejected_ApprovingItemsTerminated()
    {
        // Arrange
        var wfObj = new StartWorkflowsWithTaskApproveWorkflowObject
        {
            Items = Enumerable.Range(0, 5).Select(value => new StartWorkflowsWithTaskApproveItemWorkflowObject { Name = "Item" + value }).ToList()
        };

        // Act
        var wi = await this.StartWorkflow(wfObj);

        var startStatus = wfObj.Status;

        var waitApproveEvents = (await this.Storage.GetWaitEvents()).Where(ei => ei.Event == StartWorkflowsWithTaskApproveItemWorkflow.ApproveWaitEvent).ToList();

        var waitRejectEvents = (await this.Storage.GetWaitEvents()).Where(ei => ei.Event == StartWorkflowsWithTaskApproveItemWorkflow.RejectWaitEvent).ToList();

        var approvingInstances = waitApproveEvents.Select(e => e.TargetState.Workflow).ToArray();

        var exampleApproveEvent = waitApproveEvents.First();

        await this.Host.PushEvent(exampleApproveEvent.Event, exampleApproveEvent.TargetState);

        var exampleRejectEvent = waitRejectEvents.First(e => e.TargetState.Workflow.Source != exampleApproveEvent.TargetState.Workflow.Source);

        await this.Host.PushEvent(exampleRejectEvent.Event, exampleRejectEvent.TargetState);

        // Assert

        var usedWfObjs = new[] { exampleApproveEvent.TargetState.Workflow.Source, exampleRejectEvent.TargetState.Workflow.Source };

        var approvedWf = approvingInstances.Single(wf => wf.Source == exampleApproveEvent.TargetState.Workflow.Source);
        var rejectedWf = approvingInstances.Single(wf => wf.Source == exampleRejectEvent.TargetState.Workflow.Source);

        var notUsedApprovingInstances = approvingInstances.Where(wf => !usedWfObjs.Contains(wf.Source)).ToArray();

        startStatus.Should().Be(StartWorkflowsWithTaskApproveStatus.Approving);

        approvedWf.Status.Should().Be(WorkflowStatus.Finished);
        rejectedWf.Status.Should().Be(WorkflowStatus.Finished);

        notUsedApprovingInstances.Length.Should().Be(3);
        notUsedApprovingInstances[0].Status.Should().Be(WorkflowStatus.Terminated);
        notUsedApprovingInstances[1].Status.Should().Be(WorkflowStatus.Terminated);
        notUsedApprovingInstances[2].Status.Should().Be(WorkflowStatus.Terminated);

        wfObj.Status.Should().Be(StartWorkflowsWithTaskApproveStatus.Rejected);

        (await this.Storage.GetWaitEvents()).Should().BeEmpty();
    }

    [Fact]
    public async Task AllApproved_RootWfApproved()
    {
        // Arrange
        var wfObj = new StartWorkflowsWithTaskApproveWorkflowObject
        {
            Items = Enumerable.Range(0, 5).Select(value => new StartWorkflowsWithTaskApproveItemWorkflowObject { Name = "Item" + value }).ToList()
        };

        // Act
        var wi = await this.StartWorkflow(wfObj);

        var startStatus = wfObj.Status;

        var waitApproveEvents = (await this.Storage.GetWaitEvents()).Where(ei => ei.Event == StartWorkflowsWithTaskApproveItemWorkflow.ApproveWaitEvent).ToList();

        var approvingInstances = waitApproveEvents.Select(e => e.TargetState.Workflow).ToArray();

        foreach (var waitApproveEvent in waitApproveEvents)
        {
            await this.Host.PushEvent(waitApproveEvent.Event, waitApproveEvent.TargetState);
        }

        // Assert

        startStatus.Should().Be(StartWorkflowsWithTaskApproveStatus.Approving);

        foreach (var approvingInstance in approvingInstances)
        {
            approvingInstance.Status.Should().Be(WorkflowStatus.Finished);
        }

        wfObj.Status.Should().Be(StartWorkflowsWithTaskApproveStatus.Approved);

        (await this.Storage.GetWaitEvents()).Should().BeEmpty();
    }


    [Fact]
    public async Task TerminateChildren_RootWf_Rejected()
    {
        // Arrange
        var wfObj = new StartWorkflowsWithTaskApproveWorkflowObject
        {
            Items = Enumerable.Range(0, 5).Select(value => new StartWorkflowsWithTaskApproveItemWorkflowObject { Name = "Item" + value }).ToList()
        };

        // Act
        var wi = await this.StartWorkflow(wfObj);

        var startStatus = wfObj.Status;

        var waitApproveEvents = (await this.Storage.GetWaitEvents()).Where(ei => ei.Event == StartWorkflowsWithTaskApproveItemWorkflow.ApproveWaitEvent).ToList();

        var approvingInstances = waitApproveEvents.Select(e => e.TargetState.Workflow).ToArray();

        await this.Host.CreateMachine(approvingInstances[0]).Terminate();

        // Assert

        startStatus.Should().Be(StartWorkflowsWithTaskApproveStatus.Approving);

        foreach (var approvingInstance in approvingInstances)
        {
            approvingInstance.Status.Should().Be(WorkflowStatus.Terminated);
        }

        wfObj.Status.Should().Be(StartWorkflowsWithTaskApproveStatus.Rejected);

        (await this.Storage.GetWaitEvents()).Should().BeEmpty();
    }

    protected override IServiceCollection CreateServices()
    {
        return base.CreateServices()
            .RegisterSyncWorkflowType<StartWorkflowsWithTaskApproveItemWorkflow>();
    }
}