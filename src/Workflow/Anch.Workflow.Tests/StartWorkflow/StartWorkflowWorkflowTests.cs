using Anch.Workflow.DependencyInjection;
using Anch.Workflow.Engine;
using Anch.Workflow.Tests._Base;
using Anch.Workflow.Tests.Wait;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.Workflow.Tests.StartWorkflow;

public class StartWorkflowWorkflowTests : SingleScopeWorkflowTestBase<WaitWorkflowSource, RootWorkflow>
{
    [Fact]
    public async Task StartRootWf_SendPushEventToChild_WorkflowFinished()
    {
        // Arrange

        // Act
        var rootWi = await this.StartWorkflow(new WaitWorkflowSource());

        var allWi = await this.Storage.GetWorkflowInstances();

        var subWf = allWi.Except([rootWi]).Single();

        var preWiStatus = rootWi.Status;
        var preChildWfStatus = subWf.Status;

        var processedWorkflowInstances = await this.Host.PushEvent(new EventHeader(WaitWorkflow.WaitEventName), subWf.CurrentState, WaitWorkflow.WaitEventData);

        // Assert
        preWiStatus.Should().Be(WorkflowStatus.WaitEvent);
        preChildWfStatus.Should().Be(WorkflowStatus.WaitEvent);

        processedWorkflowInstances.Should().HaveCount(1).And.Equal(subWf);

        rootWi.Status.Should().Be(WorkflowStatus.Finished);
        subWf.Status.Should().Be(WorkflowStatus.Finished);

        (await this.Storage.GetWaitEvents()).Should().BeEmpty();
    }

    protected override IServiceCollection CreateServices()
    {
        return base.CreateServices()
            .RegisterSyncWorkflowType<WaitWorkflow>();
    }
}