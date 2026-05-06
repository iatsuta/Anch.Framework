using Anch.Testing.Xunit;
using Anch.Workflow.DependencyInjection;
using Anch.Workflow.Domain;
using Anch.Workflow.Domain.Runtime;
using Anch.Workflow.Tests._Base;
using Anch.Workflow.Tests.Wait;

namespace Anch.Workflow.Tests.StartWorkflow;

public class StartWorkflowWorkflowTests : SingleScopeWorkflowTestBase<WaitWorkflowSource, RootWorkflow>
{
    [AnchFact]
    public async ValueTask StartRootWf_SendPushEventToChild_WorkflowFinished(CancellationToken ct)
    {
        // Arrange

        // Act
        var rootWi = await this.StartWorkflow(new WaitWorkflowSource(), ct);

        var allWi = await this.RootRepository.GetWorkflowInstances().ToListAsync(ct);

        var subWf = allWi.Except([rootWi]).Single();

        var preWiStatus = rootWi.Status;
        var preChildWfStatus = subWf.Status;

        var pushResult = await this.TillTheEndWorkflowExecutor.PushEvent(new EventHeader(WaitWorkflow.WaitEventName), subWf.CurrentState, WaitWorkflow.WaitEventData, ct);

        // Assert
        Assert.Equal(WorkflowStatus.WaitEvent, preWiStatus);
        Assert.Equal(WorkflowStatus.WaitEvent, preChildWfStatus);

        Assert.Equal([subWf, rootWi], pushResult.Modified);

        Assert.Equal(WorkflowStatus.Finished, rootWi.Status);
        Assert.Equal(WorkflowStatus.Finished, subWf.Status);

        var finalEvents = await this.RootRepository.GetWaitEvents().ToListAsync(ct);

        Assert.Empty(finalEvents);
    }

    protected override void SetupWorkflow(IWorkflowSetup workflowSetup) =>

        base.SetupWorkflow(workflowSetup.Add<WaitWorkflow>());
}