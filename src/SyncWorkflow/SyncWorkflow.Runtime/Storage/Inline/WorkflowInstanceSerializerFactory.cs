using Microsoft.Extensions.DependencyInjection;

namespace SyncWorkflow.Storage.Inline;

public class WorkflowInstanceSerializerFactory(IServiceProvider serviceProvider) : IWorkflowInstanceSerializerFactory
{
    public IWorkflowInstanceSerializer<TSource> Create<TSource>(IWorkflow<TSource> workflow)
    {
        return ActivatorUtilities.CreateInstance<WorkflowInstanceSerializer<TSource>>(serviceProvider, workflow);
    }
}