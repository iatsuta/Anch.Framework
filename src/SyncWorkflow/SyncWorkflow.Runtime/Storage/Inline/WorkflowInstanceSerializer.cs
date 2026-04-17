using System.Linq.Expressions;
using CommonFramework;
using CommonFramework.ExpressionEvaluate;
using CommonFramework.IdentitySource;
using Microsoft.Extensions.DependencyInjection;
using SyncWorkflow.Definition;
using SyncWorkflow.Domain.Runtime;
using SyncWorkflow.Engine;
using SyncWorkflow.States;

namespace SyncWorkflow.Storage.Inline;

public class WorkflowInstanceSerializer<TSource>(
    IServiceProvider serviceProvider,
    IWorkflow<TSource> workflow,
    IStateInstanceSerializerFactory stateInstanceSerializerFactory,
    IIdentityInfo<TSource, Guid> identityInfo)
    : IWorkflowInstanceSerializer<TSource>
{
    private readonly IStateInstanceSerializer<TSource> stateDefinitionResolver = stateInstanceSerializerFactory.Create(workflow);

    public WorkflowInstance Deserialize(TSource source)
    {
        var currentState = stateDefinitionResolver.Deserialize(source);

        var workflowInstance = new WorkflowInstance
        {
            Definition = workflow.Definition,
            Source = source!,
            Id = identityInfo.Id.Getter(source),
            CurrentState = currentState,
            Status = this.GetWorkflowStatus(currentState.Definition)
        };

        return workflowInstance;
    }

    private WorkflowStatus GetWorkflowStatus(IStateDefinition currentStateDefinition)
    {
        var isFinished = currentStateDefinition.StateType == typeof(FinalState);
        var isTerminated = currentStateDefinition.StateType == typeof(TerminateState);

        if (isTerminated)
        {
            return WorkflowStatus.Terminated;
        }
        else if (isFinished)
        {
            return WorkflowStatus.Finished;
        }
        else if (currentStateDefinition.StateType == typeof(TaskState))
            //&& currentStateDefinition.AdditionalInfo.TryGetValue(TaskState.CommandsKey, out var untypedEventHeaders)
            //&& untypedEventHeaders is IReadOnlyList<EventHeader> eventHeaders)
        {
            //foreach (var eventHeader in eventHeaders)
            //{
            //    result.CurrentState.WaitEvents.Add(new WaitEventInfo(eventHeader, null, result.CurrentState));
            //}

            return WorkflowStatus.WaitEvent;
        }
        else
        {
            return WorkflowStatus.Runnable;
        }
    }

    public void Serialize(WorkflowInstance wi)
    {
        var source = (TSource)wi.Source;

        var stateSerializer = serviceProvider.GetRequiredService(typeof(IStateInstanceSerializer<>).MakeGenericType(wi.CurrentState.Definition.StateType));

        throw new NotImplementedException();

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

public class TerminateStateInstanceSerializer<TWorkflow, TSource, TElement> : IStateInstanceSerializer<TWorkflow, TSource, StartWorkflowsState<TSource, TElement>, TElement>
    where TWorkflow : IWorkflow<TSource>;

public class TerminateStateInstanceSerializer<TWorkflow, TSource> : IStateInstanceSerializer<TWorkflow, TSource, TerminateState, Ignore>
    where TWorkflow : IWorkflow<TSource>;

public class FinalStateInstanceSerializer<TWorkflow, TSource> : IStateInstanceSerializer<TWorkflow, TSource, FinalState, Ignore>
    where TWorkflow : IWorkflow<TSource>;

public class TaskStateInstanceSerializer<TWorkflow, TSource> : IStateInstanceSerializer<TWorkflow, TSource, TaskState, Ignore>
    where TWorkflow : IWorkflow<TSource>;

public interface IStateInstanceSerializer<TWorkflow, TSource, TState, TElement>
    where TWorkflow : IWorkflow<TSource>;

public interface IWorkflowStatusSerializer;