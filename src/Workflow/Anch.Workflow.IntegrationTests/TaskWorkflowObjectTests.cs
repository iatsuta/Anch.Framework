using Anch.GenericQueryable;
using Anch.Testing.Xunit;
using Anch.Workflow.Domain.Runtime;
using Anch.Workflow.IntegrationTests._Base;
using Anch.Workflow.Persistence;
using Anch.Workflow.Tests.TaskWorkflow;

namespace Anch.Workflow.IntegrationTests;

public abstract class TaskWorkflowObjectTests(IServiceProvider rootServiceProvider) : TestBase(rootServiceProvider)
{
    [AnchFact]
    public async Task Task_SendApproveCommand_WorkflowApproved(CancellationToken ct)
    {
        //Arrange
        var sourceId = await this.EvaluateScope(async serviceProvider =>
        {
            var source = new TaskWorkflowObject();

            await serviceProvider.GenericRepository.SaveAsync(source, ct);

            return source.Id;
        });

        //Act
        var (rootWiIdentity, firstWiStatus, firstSiIdentity, startSourceStatus) = await this.EvaluateScope(async serviceProvider =>
        {
            var source = await serviceProvider.QueryableSource.GetQueryable<TaskWorkflowObject>().Where(obj => obj.Id == sourceId).GenericSingleAsync(ct);

            var wi = await serviceProvider.StartWorkflow<TaskWorkflowObject, TaskWorkflow>(source, ct);

            return (wi.Identity, wi.Status, wi.CurrentState.Identity, source.Status);
        });

        var processedWiIdents = await this.EvaluateScope(async serviceProvider =>
        {
            var firstSi = await serviceProvider.WorkflowRepository.GetStateInstance(firstSiIdentity, ct);

            var pushResult = await serviceProvider.WorkflowExecutor.PushEvent(TaskWorkflow.ApproveEventHeader, firstSi, null, ct);

            return pushResult.Modified.Select(wi => wi.Identity).ToArray();
        });

        //Assert
        await this.EvaluateAssertScope(async serviceProvider =>
        {
            var wi = await serviceProvider.WorkflowRepository.GetWorkflowInstance(rootWiIdentity, ct);
            var source = (TaskWorkflowObject)wi.Source;

            Assert.True(source.PostProcessWork);

            Assert.Equal(TaskApproveStatus.Approving, startSourceStatus);
            Assert.Equal(TaskApproveStatus.Approved, source.Status);

            Assert.Equal(WorkflowStatus.WaitEvent, firstWiStatus);
            Assert.Equal(WorkflowStatus.Finished, wi.Status);

            Assert.Single(processedWiIdents);
            Assert.Equal(rootWiIdentity, processedWiIdents.Single());
        });
    }
}