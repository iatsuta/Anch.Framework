//using Framework.Core;

//using Microsoft.Extensions.DependencyInjection;

//using SyncWorkflow.Domain.Definition;
//using SyncWorkflow.Domain.Runtime;

//namespace SyncWorkflow.Storage.Inline;

//public class InlineWorkflowStorage : MemoryWorkflowStorage
//{
//    private readonly IServiceProvider serviceProvider;

//    private readonly IDictionaryCache<IWorkflowDefinition, IInlineSpecificWorkflowStorage> itemsCache;


//    public InlineWorkflowStorage(IServiceProvider serviceProvider)
//    {
//        this.serviceProvider = serviceProvider;

//        this.itemsCache = new DictionaryCache<IWorkflowDefinition, IInlineSpecificWorkflowStorage>(definition =>
//            (IInlineSpecificWorkflowStorage)

//                this.serviceProvider.GetRequiredService(typeof(InlineSpecificWorkflowStorage<>).MakeGenericType(definition.SourceType)));
//    }

//    protected override Guid GenerateWorkflowInstanceId(WorkflowInstance workflowInstance)
//    {
//        return this.itemsCache[workflowInstance.Definition].GenerateWorkflowInstanceId(workflowInstance);
//    }

//    public override Task<StateInstance> GetStateInstance(StateInstanceIdentity identity, CancellationToken cancellationToken = default)
//    {


//        return base.GetStateInstance(identity, cancellationToken);
//    }

//    public override Task<WorkflowInstance> GetWorkflowInstance(WorkflowInstanceIdentity identity, CancellationToken cancellationToken = default)
//    {
//        return base.GetWorkflowInstance(identity, cancellationToken);
//    }


//    public async Task FlushChanges(CancellationToken cancellationToken = default)
//    {
//        var currentSavedWorkflowInstances = this.WorkflowCache.Clone();

//        this.WorkflowCache.Clear();

//        foreach (var wfInfo in currentSavedWorkflowInstances.Values)
//        {
//            await this.itemsCache[wfInfo.WorkflowInstance.Definition].SaveWorkflowInstance(wfInfo, cancellationToken);
//        }
//    }
//}