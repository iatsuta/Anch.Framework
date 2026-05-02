using Anch.Workflow.Domain;
using Anch.Workflow.Domain.Runtime;

namespace Anch.Workflow;

public interface IWorkflowExecutor
{
    Task<WorkflowProcessResult> StartWorkflow<TSource, TWorkflow>(TSource source, CancellationToken cancellationToken = default)
        where TSource : notnull
        where TWorkflow : IWorkflow<TSource>;

    Task<WorkflowProcessResult> StartWorkflow<TSource>(TSource source, IWorkflow<TSource> workflow, CancellationToken cancellationToken = default)
        where TSource : notnull;

    async Task<WorkflowProcessResult> PushEvent(EventHeader @event, StateInstance targetState, object? data = null,
        CancellationToken cancellationToken = default)
    {
        return await this.PushEvent(new PushEventInfo(@event, null, targetState, data), cancellationToken);
    }

    Task<WorkflowProcessResult> PushEvent(PushEventInfo pushEventInfo, CancellationToken cancellationToken = default);
}