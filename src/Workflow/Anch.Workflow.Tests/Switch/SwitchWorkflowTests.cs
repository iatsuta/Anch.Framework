using Anch.Workflow.Engine;
using Anch.Workflow.Tests._Base;

namespace Anch.Workflow.Tests.Switch;

public class SwitchWorkflowTests : SingleScopeWorkflowTestBase<SwitchWorkflowObject, SwitchWorkflow>
{
    [Theory]
    [InlineData(0, "0 DefaultCase")]
    [InlineData(1, "1 Case 1")]
    [InlineData(2, "2 Case 2")]
    [InlineData(3, "3 Case 3")]
    [InlineData(4, "4 DefaultCase")]
    public async Task UseIf_CodeSwitched_Cases(int value, string expectedResult)
    {
        // Arrange
        var wfObj = new SwitchWorkflowObject { Value = value };

        // Act
        var wi = await this.StartWorkflow(wfObj);

        // Assert
        wi.Status.Should().Be(WorkflowStatus.Finished);
        wfObj.Result.Should().Be(expectedResult);

        (await this.Storage.GetWaitEvents()).Should().BeEmpty();
    }
}