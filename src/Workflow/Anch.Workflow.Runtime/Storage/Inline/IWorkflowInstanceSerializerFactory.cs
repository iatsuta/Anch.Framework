namespace Anch.Workflow.Storage.Inline;

public interface IWorkflowInstanceSerializerFactory
{
    IWorkflowInstanceSerializer<TSource> Create<TSource>(IWorkflow<TSource> workflow);
}