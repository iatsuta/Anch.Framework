using Anch.IdentitySource;
using Anch.Workflow.Domain.Definition;
using Anch.Workflow.Domain.Runtime;

namespace Anch.Workflow.Serialization.Inline;

public class WorkflowInstanceSerializer<TSource>(
    IServiceProvider serviceProvider,
    IWorkflowDefinition workflow,
    IStateInstanceSerializerFactory stateInstanceSerializerFactory,
    IIdentityInfo<TSource, Guid> identityInfo)
    : IWorkflowInstanceSerializer
    where TSource : class
{
    private readonly IStateInstanceSerializer stateInstanceSerializer = stateInstanceSerializerFactory.Create(workflow);

    public WorkflowInstance Deserialize(object source)
    {
        var typeSource = (TSource)source;

        var currentState = this.stateInstanceSerializer.Deserialize(source);

        var workflowInstance = new WorkflowInstance
        {
            Definition = workflow,
            Source = source!,
            Id = identityInfo.Id.Getter(typeSource),
            CurrentState = currentState,
            Status = this.GetWorkflowStatus(currentState.Definition)
        };

        return workflowInstance;
    }

    private WorkflowStatus GetWorkflowStatus(IStateDefinition currentStateDefinition)
    {
        throw new NotImplementedException();

        //var isFinished = currentStateDefinition.StateType == typeof(FinalState);
        //var isTerminated = currentStateDefinition.StateType == typeof(TerminateState);

        //if (isTerminated)
        //{
        //    return WorkflowStatus.Terminated;
        //}
        //else if (isFinished)
        //{
        //    return WorkflowStatus.Finished;
        //}
        //else if (currentStateDefinition.StateType == typeof(TaskState))
        //    //&& currentStateDefinition.AdditionalInfo.TryGetValue(TaskState.CommandsKey, out var untypedEventHeaders)
        //    //&& untypedEventHeaders is IReadOnlyList<EventHeader> eventHeaders)
        //{
        //    //foreach (var eventHeader in eventHeaders)
        //    //{
        //    //    result.CurrentState.WaitEvents.Add(new WaitEventInfo(eventHeader, null, result.CurrentState));
        //    //}

        //    return WorkflowStatus.WaitEvent;
        //}
        //else
        //{
        //    return WorkflowStatus.Runnable;
        //}
    }

    public void Serialize(WorkflowInstance wi)
    {
        var source = (TSource)wi.Source;

        //this.workflowInfoSerializer.SetInfo(source, new InlineWorkflowInfo
        //{
        //    IsTerminated = wi.Status == WorkflowStatus.Terminated,
        //    IsFinished = wi.Status == WorkflowStatus.Finished,
        //});

        //if (Enum.TryParse<TaskApproveStatus>(wi.CurrentState.Definition.Name, out var status))
        //{
        //    source.Status = status;
        //}
    }
}

//public class TerminateStateInstanceSerializer<TWorkflow, TSource, TElement> : IStateInstanceSerializer<TWorkflow, TSource, StartWorkflowsState<TSource, TElement>, TElement>
//    where TWorkflow : IWorkflow<TSource>;

//public class TerminateStateInstanceSerializer<TWorkflow, TSource> : IStateInstanceSerializer<TWorkflow, TSource, TerminateState, Ignore>
//    where TWorkflow : IWorkflow<TSource>;

//public class FinalStateInstanceSerializer<TWorkflow, TSource> : IStateInstanceSerializer<TWorkflow, TSource, FinalState, Ignore>
//    where TWorkflow : IWorkflow<TSource>;

//public class TaskStateInstanceSerializer<TWorkflow, TSource> : IStateInstanceSerializer<TWorkflow, TSource, TaskState, Ignore>
//    where TWorkflow : IWorkflow<TSource>;

//public interface IStateInstanceSerializer<TWorkflow, TSource, TState, TElement>
//    where TWorkflow : IWorkflow<TSource>;

//public interface IWorkflowStatusSerializer;