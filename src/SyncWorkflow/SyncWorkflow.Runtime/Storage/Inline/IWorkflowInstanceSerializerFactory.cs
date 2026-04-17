namespace SyncWorkflow.Storage.Inline;

public interface IWorkflowInstanceSerializerFactory
{
    IWorkflowInstanceSerializer<TSource> Create<TSource>(IWorkflow<TSource> workflow);
}