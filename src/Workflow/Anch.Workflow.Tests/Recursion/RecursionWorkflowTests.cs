using Anch.Workflow.Engine;
using Anch.Workflow.Tests._Base;

namespace Anch.Workflow.Tests.Recursion;

public class RecursionWorkflowTests : SingleScopeWorkflowTestBase<RecursionWorkflowObject, RecursionWorkflow>
{
    [Theory]
    [InlineData(4, 1000, 1006)]
    [InlineData(11, 1000, 1055)]
    public async Task SumNumbersByRecurse_ResultEquals(int limit, int extraAddToResult, int result)
    {
        // Arrange
        var wfObj = new RecursionWorkflowObject { Limit = limit, ExtraAddToResult = extraAddToResult };

        // Act
        var wi = await this.StartWorkflow(wfObj);

        // Assert
        wi.Status.Should().Be(WorkflowStatus.Finished);
        wfObj.Result.Should().Be(result);

        (await this.Storage.GetWaitEvents()).Should().BeEmpty();
    }
}