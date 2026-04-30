using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using NHibernate;

using SyncWorkflow.DependencyInjection;
using SyncWorkflow.Engine;
using SyncWorkflow.NHibernate;
using SyncWorkflow.Storage.Inline;
using SyncWorkflow.Tests;
using SyncWorkflow.Tests._TaskState;

namespace SyncWorkflow.IntegrationTests;

public class TaskWorkflowObjectTests : InlineWorkflowTestBase
{
    [Fact]
    public async Task Task_SendApproveCommand_WorkflowApproved()
    {
        //Arrange
        var sourceId = await this.EvaluateScope(async scope =>
        {
            var session = scope.ServiceProvider.GetRequiredService<ISession>();

            var source = new TaskWorkflowObject();

            await session.SaveAsync(source);

            return source.Id;
        });

        //Act
        var (rootWiIdentity, firstWiStatus, firstSiIdentity, startSourceStatus) = await this.EvaluateScope(async scope =>
        {
            var session = scope.ServiceProvider.GetRequiredService<ISession>();

            var source = await session.GetAsync<TaskWorkflowObject>(sourceId);

            var wi = await scope.StartWorkflow<TaskWorkflowObject, TaskWorkflow>(source);

            return (wi.Identity, wi.Status, wi.CurrentState.Identity, source.Status);
        });

        var processedWiIdents = await this.EvaluateScope(async scope =>
        {
            var firstSi = await scope.Storage.GetStateInstance(firstSiIdentity);

            var pushResult = await scope.Host.PushEvent(TaskWorkflow.ApproveEventHeader, firstSi);

            return pushResult;
        });

        //Assert
        await this.EvaluateAssertScope(async scope =>
        {
            var wi = await scope.Storage.GetWorkflowInstance(rootWiIdentity);
            var source = (TaskWorkflowObject)wi.Source;

            source.PostProcessWork.Should().BeTrue();

            startSourceStatus.Should().Be(TaskApproveStatus.Approving);
            source.Status.Should().Be(TaskApproveStatus.Approved);

            firstWiStatus.Should().Be(WorkflowStatus.WaitEvent);
            wi.Status.Should().Be(WorkflowStatus.Finished);

            processedWiIdents.Count.Should().Be(1);
            processedWiIdents[0].Identity.Should().Be(rootWiIdentity);
        });
    }

    protected override IServiceCollection CreateServices()
    {
        return base.CreateServices()

            .RegisterSyncWorkflowType<TaskWorkflow>()

            //.AddScoped<InlineWorkflowStorageItem<TaskWorkflowObject>, TaskWorkflowInlineWorkflowStorageItem>()
            .AddScoped<IWorkflowInstanceSerializerFactory, WorkflowInstanceSerializerFactory>()
            .AddScoped<IStateInstanceSerializerFactory, StateInstanceSerializerFactory>()

            .AddScoped(typeof(IInlineStorage<>), typeof (NHibInlineStorage<>))

            .AddSingleton<IStateDefinitionResolverFactory, StateDefinitionResolverFactory>();
    }
}