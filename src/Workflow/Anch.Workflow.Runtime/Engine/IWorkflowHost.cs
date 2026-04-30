using Anch.Workflow.Definition;
using Anch.Workflow.Domain.Runtime;

namespace Anch.Workflow.Engine;

public interface IWorkflowExecutor
{
    Task<WorkflowProcessResult> StartWorkflow<TSource, TWorkflow>(TSource source, CancellationToken cancellationToken = default)
        where TSource : notnull
        where TWorkflow : IWorkflow<TSource>;

    Task<WorkflowProcessResult> StartWorkflow<TSource, TWorkflow>(TSource source, TWorkflow workflow, CancellationToken cancellationToken = default)
        where TSource : notnull
        where TWorkflow : IWorkflow<TSource>;

    async Task<WorkflowProcessResult> PushEvent(EventHeader @event, StateInstance targetState, object? data = null, CancellationToken cancellationToken = default)
    {
        return await this.PushEvent(new PushEventInfo(@event, null, targetState, data), cancellationToken);
    }

    Task<WorkflowProcessResult> PushEvent(PushEventInfo pushEventInfo, CancellationToken cancellationToken = default);
}

public interface IWorkflowHost
{
    IWorkflowExecutor CreateExecutor(WorkflowExecutionPolicy executionPolicy);

    IWorkflowMachine CreateMachine<TSource, TWorkflow>(TSource source)
        where TSource : notnull
        where TWorkflow : IWorkflow<TSource>;

    IWorkflowMachine CreateMachine<TSource, TWorkflow>(TSource source, TWorkflow workflow)
        where TSource : notnull
        where TWorkflow : IWorkflow<TSource>;

    IWorkflowMachine CreateMachine(object source, IWorkflowDefinition workflowDefinition);

    IWorkflowMachine CreateMachine(WorkflowInstance workflowInstance);
}