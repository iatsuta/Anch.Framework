using CommonFramework.DictionaryCache;
using Microsoft.Extensions.DependencyInjection;
using SyncWorkflow.Domain.Runtime;
using SyncWorkflow.Storage;

namespace SyncWorkflow.Engine;

public class WorkflowMachineFactory : IWorkflowMachineFactory
{
    private readonly IDictionaryCache<WorkflowInstance, IWorkflowMachine> cache;

    public WorkflowMachineFactory(IServiceProvider serviceProvider, IWorkflowStorage workflowStorage)
    {
        this.cache = new DictionaryCache<WorkflowInstance, IWorkflowMachine>(wi =>

            (IWorkflowMachine)ActivatorUtilities.CreateInstance(
                serviceProvider,
                typeof(WorkflowMachine<>).MakeGenericType(wi.Definition.SourceType),
                workflowStorage.GetSpecificStorage(wi.Definition.Identity),
                wi));
    }

    public IWorkflowMachine Create(WorkflowInstance workflowInstance)
    {
        return this.cache[workflowInstance];
    }
}