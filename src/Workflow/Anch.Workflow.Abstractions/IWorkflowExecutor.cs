using Anch.Workflow.Domain;
using Anch.Workflow.Domain.Definition;
using Anch.Workflow.Domain.Runtime;
using Anch.Workflow.Execution;

namespace Anch.Workflow;

public interface IWorkflowExecutor
{
    ValueTask<WorkflowProcessResult> Start<TSource, TWorkflow>(TSource source, CancellationToken cancellationToken = default)
        where TSource : notnull
        where TWorkflow : IWorkflow<TSource>;

    ValueTask<WorkflowProcessResult> Start<TSource>(TSource source, IWorkflow<TSource> workflow, CancellationToken cancellationToken = default)
        where TSource : notnull;

    ValueTask<WorkflowProcessResult> Start<TSource>(TSource source, IWorkflowDefinition<TSource> workflow, CancellationToken cancellationToken = default)
        where TSource : notnull;

    ValueTask<WorkflowProcessResult> ProcessUnprocessed(WorkflowProcessResult workflowProcessResult, CancellationToken cancellationToken = default);

    ValueTask<WorkflowProcessResult> Terminate(WorkflowInstance workflowInstance, CancellationToken cancellationToken = default);

    async ValueTask<WorkflowProcessResult> PushEvent(EventHeader @event, StateInstance targetState, object? data = null,
        CancellationToken cancellationToken = default) =>

        await this.PushEvent(new PushEventInfo(@event, null, targetState, data), cancellationToken);

    ValueTask<WorkflowProcessResult> PushEvent(PushEventInfo pushEventInfo, CancellationToken cancellationToken = default);
}