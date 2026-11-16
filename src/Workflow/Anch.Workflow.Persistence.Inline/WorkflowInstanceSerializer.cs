using Anch.IdentitySource;
using Anch.Workflow.Domain.Definition;
using Anch.Workflow.Domain.Runtime;
using Anch.Workflow.States;

namespace Anch.Workflow.Persistence.Inline;

public class WorkflowInstanceSerializer<TSource, TStatus>(
    IWorkflowDefinition<TSource, TStatus> workflow,
    IStateInstanceSerializerFactory<TSource, TStatus> stateInstanceSerializerFactory,
    IIdentityInfo<TSource, Guid> identityInfo)
    : IWorkflowInstanceSerializer<TSource>
    where TSource : class
    where TStatus : struct
{
    private readonly IStateInstanceSerializer stateInstanceSerializer = stateInstanceSerializerFactory.Create(workflow);

    public WorkflowInstance Deserialize(TSource source)
    {
        var workflowInstance = new WorkflowInstance
        {
            Definition = workflow,
            Source = source,
            Id = identityInfo.Id.Getter(source),
            Status = WorkflowStatus.NotStarted
        };

        var currentState = this.stateInstanceSerializer.Deserialize(workflowInstance);

        workflowInstance.CurrentState = currentState;
        workflowInstance.Status = GetWorkflowStatus(currentState.Definition);

        return workflowInstance;
    }

    /// <summary>
    /// The status enum is written to the domain object by the engine on state entry; the state data is written by the state serializer.
    /// </summary>
    public void Serialize(WorkflowInstance wi) => this.stateInstanceSerializer.Serialize(wi.CurrentState);

    private static WorkflowStatus GetWorkflowStatus(IStateDefinition currentStateDefinition) =>

        currentStateDefinition.StateType == typeof(TerminateState) ? WorkflowStatus.Terminated
        : currentStateDefinition.StateType == typeof(FinalState) ? WorkflowStatus.Finished
        : currentStateDefinition.StateType == typeof(TaskState) ? WorkflowStatus.WaitEvent
        : WorkflowStatus.Runnable;
}
