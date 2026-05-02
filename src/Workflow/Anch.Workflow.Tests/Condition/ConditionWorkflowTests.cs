using Anch.Testing.Xunit;
using Anch.Workflow.Domain.Runtime;
using Anch.Workflow.Tests._Base;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.Workflow.Tests.Condition;

public class ConditionWorkflowTests : SingleScopeWorkflowTestBase<ConditionWorkflowObject, ConditionWorkflow>
{
    [Theory]
    [AnchInlineData(100, true)]
    [AnchInlineData(101, false)]
    public async Task UseIf_CodeSwitched_Cases(int value, bool isEven, CancellationToken ct)
    {
        // Arrange
        var wfObj = new ConditionWorkflowObject { Value = value };

        var expectedResult = ConditionWorkflow.BuildResult(wfObj.Value, isEven);

        // Act
        var wi = await this.StartWorkflow(wfObj, ct);

        // Assert
        Assert.Equal(WorkflowStatus.Finished, wi.Status);
        Assert.Equal(expectedResult, wfObj.Result);

        Assert.Empty(await this.Storage.GetWaitEvents(ct));
    }

    protected override IServiceCollection CreateServices(IServiceCollection services)
    {
        return base.CreateServices(services)
            .AddSingleton<ConditionWorkflowService>();
    }
}