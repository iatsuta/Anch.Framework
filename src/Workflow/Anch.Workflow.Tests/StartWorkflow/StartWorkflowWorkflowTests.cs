using Anch.Testing.Xunit;
using Anch.Workflow.DependencyInjection;
using Anch.Workflow.Domain;
using Anch.Workflow.Domain.Runtime;
using Anch.Workflow.Tests._Base;
using Anch.Workflow.Tests.StartWorkflowsWithForksApprove;
using Anch.Workflow.Tests.Wait;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.Workflow.Tests.StartWorkflow;

public class StartWorkflowWorkflowTests : SingleScopeWorkflowTestBase<WaitWorkflowSource, RootWorkflow>
{
    [AnchFact]
    public async Task StartRootWf_SendPushEventToChild_WorkflowFinished(CancellationToken ct)
    {
        // Arrange

        // Act
        var rootWi = await this.StartWorkflow(new WaitWorkflowSource(), ct);

        var allWi = await this.Storage.GetWorkflowInstances(ct);

        var subWf = allWi.Except([rootWi]).Single();

        var preWiStatus = rootWi.Status;
        var preChildWfStatus = subWf.Status;

        var pushResult = await this.Host
            .CreateExecutor(WorkflowExecutionPolicy.Full)
            .PushEvent(new EventHeader(WaitWorkflow.WaitEventName), subWf.CurrentState, WaitWorkflow.WaitEventData, ct);

        // Assert
        Assert.Equal(WorkflowStatus.WaitEvent, preWiStatus);
        Assert.Equal(WorkflowStatus.WaitEvent, preChildWfStatus);

        Assert.Single(pushResult.Started);
        Assert.Contains(subWf, pushResult.Started);

        Assert.Equal(WorkflowStatus.Finished, rootWi.Status);
        Assert.Equal(WorkflowStatus.Finished, subWf.Status);

        Assert.Empty(await this.Storage.GetWaitEvents(ct));
    }

    protected override void SetupWorkflow(IWorkflowSetup workflowSetup)
    {
        base.SetupWorkflow(workflowSetup);

        workflowSetup.Add<WaitWorkflow>();
    }
}