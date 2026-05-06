using Anch.Testing.Xunit;
using Anch.Workflow.DependencyInjection;
using Anch.Workflow.Domain.Runtime;
using Anch.Workflow.Tests._Base;

namespace Anch.Workflow.Tests.ParallelForeach;

public class ParallelForeachWorkflowTests : SingleScopeWorkflowTestBase<ParallelForeachWorkflowObject, ParallelForeachWorkflow>
{
    [Theory]
    [AnchInlineData(10, 100, 123, 760)]
    [AnchInlineData(3, 100, 5, 113)]
    public async ValueTask ParallelSum_ResultEquals(int wfCount, int extraAddToResult, int pushEventData, int expectedResult, CancellationToken ct)
    {
        // Arrange
        var wfObj = new ParallelForeachWorkflowObject
        {
            Items = [.. Enumerable.Range(0, wfCount).Select(value => new ParallelForeachItemWorkflowObject { Value = value, Name = "Item" + value })],
            ExtraAddToResult = extraAddToResult
        };

        // Act
        var wi = await this.StartWorkflow(wfObj, ct);

        var preWiStatus = wi.Status;

        var waitUserEvents = await this.RootRepository.GetWaitEvents().Where(ei => ei.Header == ParallelForeachItemWorkflow.TestItemWaitEvent).ToListAsync(ct);

        foreach (var waitUserEvent in waitUserEvents)
        {
            await this.TillTheEndWorkflowExecutor.PushEvent(waitUserEvent.Header, waitUserEvent.TargetState, pushEventData, ct);
        }

        // Assert
        Assert.Equal(WorkflowStatus.WaitEvent, preWiStatus);

        Assert.Equal((wfCount + 1) / 2, waitUserEvents.Count);
        Assert.Equal(WorkflowStatus.Finished, wi.Status);
        Assert.Equal(expectedResult, wfObj.Result);

        var finalEvents = await this.RootRepository.GetWaitEvents().ToListAsync(ct);

        Assert.Empty(finalEvents);
    }

    protected override void SetupWorkflow(IWorkflowSetup workflowSetup) =>

        base.SetupWorkflow(workflowSetup.Add<ParallelForeachItemWorkflow>());
}