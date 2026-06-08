using Anch.Workflow.Domain;
using Anch.Workflow.Domain.Definition;
using Anch.Workflow.Domain.Runtime;
using Anch.Workflow.Execution;

namespace Anch.Workflow;

public interface IWorkflowExecutor
{
    ValueTask<WorkflowProcessResult> Start<TSource, TWorkflow>(TSource source, CancellationToken ct)
        where TSource : notnull
        where TWorkflow : IWorkflow<TSource>;

    ValueTask<WorkflowProcessResult> Start<TSource>(TSource source, IWorkflow<TSource> workflow, CancellationToken ct)
        where TSource : notnull;

    ValueTask<WorkflowProcessResult> Start<TSource>(TSource source, IWorkflowDefinition<TSource> workflow, CancellationToken ct)
        where TSource : notnull;

    ValueTask<WorkflowProcessResult> ProcessUnprocessed(WorkflowProcessResult workflowProcessResult, CancellationToken ct);

    ValueTask<WorkflowProcessResult> Terminate(WorkflowInstance workflowInstance, CancellationToken ct);

    async ValueTask<WorkflowProcessResult> PushEvent(EventHeader @event, StateInstance targetState, object? data,
        CancellationToken ct) =>

        await this.PushEvent(new PushEventInfo(@event, null, targetState, data), ct);

    ValueTask<WorkflowProcessResult> PushEvent(PushEventInfo pushEventInfo, CancellationToken ct);
}