using SyncWorkflow.Domain.Runtime;

namespace SyncWorkflow.Engine;

public class ExecutionContext : IExecutionContext
{
    public StateInstance StateInstance { get; init; } = null!;

    public CancellationToken CancellationToken { get; init; } = CancellationToken.None!;

    public WaitEventInfo? CallbackEventInfo { get; init; } = null;


    public object Source => this.WorkflowInstance.Source;

    public WorkflowInstance WorkflowInstance => this.StateInstance.Workflow;

    public bool IsActual => this.StateInstance.IsActual;
}