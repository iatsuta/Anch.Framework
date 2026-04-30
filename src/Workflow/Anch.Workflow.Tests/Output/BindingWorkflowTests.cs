using Anch.Workflow.Engine;
using Anch.Workflow.Tests._Base;

namespace Anch.Workflow.Tests.Output;

public class BindingWorkflowTests : SingleScopeWorkflowTestBase<BindingWorkflowObject, BindingWorkflow>
{
    [Theory]
    [InlineData(2, 3, 5)]
    [InlineData(6, 7, 13)]
    public async System.Threading.Tasks.Task BindingInputAndOutputPropsByState_ResultEquals(int v1, int v2, int expectedResult)
    {
        // Arrange
        var ct = TestContext.Current.CancellationToken;
        var wfObj = new BindingWorkflowObject { Value1 = v1, Value2 = v2 };

        // Act
        var wi = await this.StartWorkflow(wfObj, ct);

        // Assert
        wi.Status.Should().Be(WorkflowStatus.Finished);
        wfObj.Result.Should().Be(expectedResult);

        (await this.Storage.GetWaitEvents(ct)).Should().BeEmpty();
    }
}