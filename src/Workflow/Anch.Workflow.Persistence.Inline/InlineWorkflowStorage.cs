//using Framework.Core;

//using Microsoft.Extensions.DependencyInjection;

//using SyncWorkflow.Domain.Definition;
//using SyncWorkflow.Domain.Runtime;

//namespace SyncWorkflow.Storage.Inline;

//public class InlineWorkflowRepository : MemoryWorkflowRepository
//{
//    private readonly IServiceProvider serviceProvider;

//    private readonly IDictionaryCache<IWorkflowDefinition, IInlineSpecificWorkflowRepository> itemsCache;


//    public InlineWorkflowRepository(IServiceProvider serviceProvider)
//    {
//        this.serviceProvider = serviceProvider;

//        this.itemsCache = new DictionaryCache<IWorkflowDefinition, IInlineSpecificWorkflowRepository>(definition =>
//            (IInlineSpecificWorkflowRepository)

//                this.serviceProvider.GetRequiredService(typeof(InlineSpecificWorkflowRepository<>).MakeGenericType(definition.SourceType)));
//    }

//    protected override Guid GenerateWorkflowInstanceId(WorkflowInstance workflowInstance)
//    {
//        return this.itemsCache[workflowInstance.Definition].GenerateWorkflowInstanceId(workflowInstance);
//    }

//    public override ValueTask<StateInstance> GetStateInstance(StateInstanceIdentity identity, CancellationToken cancellationToken)
//    {


//        return base.GetStateInstance(identity, cancellationToken);
//    }

//    public override ValueTask<WorkflowInstance> GetWorkflowInstance(WorkflowInstanceIdentity identity, CancellationToken cancellationToken)
//    {
//        return base.GetWorkflowInstance(identity, cancellationToken);
//    }


//    public async ValueTask FlushChanges(CancellationToken cancellationToken)
//    {
//        var currentSavedWorkflowInstances = this.WorkflowCache.Clone();

//        this.WorkflowCache.Clear();

//        foreach (var wfInfo in currentSavedWorkflowInstances.Values)
//        {
//            await this.itemsCache[wfInfo.WorkflowInstance.Definition].SaveWorkflowInstance(wfInfo, cancellationToken);
//        }
//    }
//}