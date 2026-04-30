using Anch.Workflow.DependencyInjection;
using Anch.Workflow.Engine;
using Anch.Workflow.Tests._Base;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.Workflow.Tests.ParallelForeach;

public class ParallelForeachWorkflowTests : SingleScopeWorkflowTestBase<ParallelForeachWorkflowObject, ParallelForeachWorkflow>
{
    [Theory]
    [InlineData(10, 100, 123, 760)]
    [InlineData(3, 100, 5, 113)]
    public async Task ParallelSum_ResultEquals(int wfCount, int extraAddToResult, int pushEventData, int expectedResult)
    {
        // Arrange
        var wfObj = new ParallelForeachWorkflowObject
        {
            Items = Enumerable.Range(0, wfCount).Select(value => new ParallelForeachItemWorkflowObject { Value = value, Name = "Item" + value }).ToList(),
            ExtraAddToResult = extraAddToResult
        };

        // Act
        var wi = await this.StartWorkflow(wfObj);

        var preWiStatus = wi.Status;

        var waitUserEvents = (await this.Storage.GetWaitEvents()).Where(ei => ei.Event == ParallelForeachItemWorkflow.TestItemWaitEvent).ToList();

        foreach (var waitUserEvent in waitUserEvents)
        {
            await this.Host.PushEvent(waitUserEvent.Event, waitUserEvent.TargetState, pushEventData);
        }

        // Assert
        preWiStatus.Should().Be(WorkflowStatus.WaitEvent);

        waitUserEvents.Count.Should().Be((wfCount + 1) / 2);
        wi.Status.Should().Be(WorkflowStatus.Finished);
        wfObj.Result.Should().Be(expectedResult);

        (await this.Storage.GetWaitEvents()).Should().BeEmpty();
    }

    protected override IServiceCollection CreateServices()
    {
        return base.CreateServices()
            .RegisterSyncWorkflowType<ParallelForeachItemWorkflow>();
    }
}