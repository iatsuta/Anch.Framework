//using FluentAssertions;

//using Microsoft.Extensions.DependencyInjection;

//using Framework.Core;
//using Framework.DependencyInjection;
//using SyncWorkflow.Domain.Runtime;
//using SyncWorkflow.Engine;
//using SyncWorkflow.Storage;
//using SyncWorkflow.Storage.Inline;

//namespace SyncWorkflow.Tests.Wait;

//public class WaitWorkflowWithInlineStorageTests : MultiScopeWorkflowTestBase
//{
//    [Fact]
//    public async Task SendWaitedEvent_TestPassed()
//    {
//        // Arrange
//        WorkflowInstanceIdentity rootWorkflowInstanceInstanceIdentity;
//        WorkflowStatus preWiStatus;
//        StateInstanceIdentity waitStateInstanceIdentity;
//        List<WorkflowInstanceIdentity> processedWorkflowInstancesInstanceIdentities;

//        var sourceId = Guid.NewGuid();

//        // Act
//        await using (var startScope = this.RootServiceProvider.CreateAsyncScope())
//        {
//            var source = new WaitWorkflowSource { Id = sourceId };
//            var wi = await startScope.ServiceProvider.StartWorkflow<WaitWorkflowSource, WaitWorkflow>(source);

//            rootWorkflowInstanceInstanceIdentity = wi.Identity;
//            preWiStatus = wi.Status;
//            waitStateInstanceIdentity = wi.CurrentState.Identity;

//            await startScope.ServiceProvider.FlushInline();
//        }

//        await using (var pushScope = this.RootServiceProvider.CreateAsyncScope())
//        {
//            var waitStateInstance = await pushScope.ServiceProvider.GetWorkflowStorage().GetStateInstance(waitStateInstanceIdentity);

//            var processedWorkflowInstances = await pushScope.ServiceProvider.GetWorkflowHost().PushEvent(new EventHeader(WaitWorkflow.WaitEventName), waitStateInstance, WaitWorkflow.WaitEventData);

//            processedWorkflowInstancesInstanceIdentities = processedWorkflowInstances.ToList(v => v.Identity);

//            await pushScope.ServiceProvider.FlushInline();
//        }

//        // Assert
//        await using (var assertScope = this.RootServiceProvider.CreateAsyncScope())
//        {
//            var storage = assertScope.ServiceProvider.GetWorkflowStorage();

//            preWiStatus.Should().Be(WorkflowStatus.WaitEvent);

//            processedWorkflowInstancesInstanceIdentities.Should().Contain(rootWorkflowInstanceInstanceIdentity);

//            var wi = await storage.GetWorkflowInstance(rootWorkflowInstanceInstanceIdentity);
//            wi.Status.Should().Be(WorkflowStatus.Finished);

//            storage.GetWaitEvents().Should().BeEmpty();
//        }
//    }

//    protected override IServiceCollection CreateServices()
//    {
//        return base.CreateServices()

//            .AddScoped<InlineWorkflowStorage>()
//            .ReplaceScopedFrom<IWorkflowStorage, InlineWorkflowStorage>()

//            .AddScoped<InlineWorkflowStorageItem<WaitWorkflowSource>, WaitWorkflowInlineWorkflowStorageItem>()
//            .AddScoped<IWorkflowInstanceSerializer<WaitWorkflowSource>, WaitWorkflowWorkflowSerializer>()
//            .AddScoped<IInlineStorage<WaitWorkflowSource>, WaitWorkflowPersistSource>();
//    }
//}

//public class WaitWorkflowInlineWorkflowStorageItem : InlineWorkflowStorageItem<WaitWorkflowSource>
//{
//    public WaitWorkflowInlineWorkflowStorageItem(
//        IWorkflowInstanceSerializer<WaitWorkflowSource> mappingService,
//        IInlineStorage<WaitWorkflowSource> persistSource)
//        : base(mappingService, persistSource)
//    {
//    }

//    public override WorkflowInstanceIdentity GenerateWorkflowInstanceIdentity(WorkflowInstance workflowInstance)
//    {
//        var source = (WaitWorkflowSource)workflowInstance.Source;

//        return new WorkflowInstanceIdentity(source.Id);
//    }
//}

//public class WaitWorkflowWorkflowSerializer : IWorkflowInstanceSerializer<WaitWorkflowSource>
//{
//    public WorkflowInstance Deserialize(WaitWorkflowSource source)
//    {
//        throw new NotImplementedException();
//    }

//    public void Serialize(WorkflowInstance workflowInstance)
//    {
//        var source = (WaitWorkflowSource)workflowInstance.Source;

//        var isTerminated = workflowInstance.Status == WorkflowStatus.Terminated;

//        source.WorkflowInfo.IsTerminated = isTerminated;

//        if (!isTerminated)
//        {
//            source.Status = Enum.Parse<WaitWorkflowStatus>(workflowInstance.CurrentState.Definition.Name);
//        }
//    }
//}

//public class WaitWorkflowPersistSource : IInlineStorage<WaitWorkflowSource>
//{
//    public Task Save(WaitWorkflowSource source, CancellationToken cancellationToken = default)
//    {
//        throw new NotImplementedException();
//    }
//}