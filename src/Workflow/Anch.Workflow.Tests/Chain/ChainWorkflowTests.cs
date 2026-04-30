using Anch.Workflow.Engine;
using Anch.Workflow.Tests._Base;

namespace Anch.Workflow.Tests.Chain;

public class ChainWorkflowTests : SingleScopeWorkflowTestBase<ChainWorkflowObject, ChainWorkflow>
{
    [Theory]
    [InlineData(2, 3, 5)]
    [InlineData(6, 7, 13)]
    public async Task ChainNumbers_ResultEquals(int v1, int v2, int expectedResult)
    {
        // Arrange
        var wfObj = new ChainWorkflowObject { Value1 = v1, Value2 = v2 };

        // Act
        var wi = await this.StartWorkflow(wfObj);

        // Assert
        wi.Status.Should().Be(WorkflowStatus.Finished);
        wfObj.Result.Should().Be(expectedResult);

        (await this.Storage.GetWaitEvents()).Should().BeEmpty();
    }
}