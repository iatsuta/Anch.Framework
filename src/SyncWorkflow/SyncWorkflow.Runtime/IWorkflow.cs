using SyncWorkflow.Domain.Definition;

namespace SyncWorkflow;

public interface IWorkflow
{
    IWorkflowDefinition Definition { get; }
}

public interface IWorkflow<out TSource> : IWorkflow
{
}