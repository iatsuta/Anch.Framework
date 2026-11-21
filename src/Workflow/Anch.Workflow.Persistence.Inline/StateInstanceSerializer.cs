using Anch.Workflow.Domain.Definition;
using Anch.Workflow.Domain.Runtime;
using Anch.Workflow.States;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.Workflow.Persistence.Inline;

public class StateInstanceSerializer<TSource, TStatus>(
    IServiceProvider serviceProvider,
    IWorkflowDefinition<TSource, TStatus> workflowDefinition,
    IStateDefinitionResolverFactory stateDefinitionResolverFactory) : IStateInstanceSerializer
    where TSource : class
    where TStatus : struct
{
    private readonly IStateDefinitionResolver<TSource, TStatus> stateDefinitionResolver = stateDefinitionResolverFactory.Create(workflowDefinition);

    public StateInstance Deserialize(WorkflowInstance workflowInstance)
    {
        var source = (TSource)workflowInstance.Source;

        var currentStateDefinition = this.stateDefinitionResolver.GetCurrentStateDefinition(source);

        var isFinal = currentStateDefinition.StateType == typeof(FinalState) || currentStateDefinition.StateType == typeof(TerminateState);

        var stateInstance = new StateInstance
        {
            Id = workflowInstance.Id,
            Workflow = workflowInstance,
            Definition = currentStateDefinition,
            InputProcessed = isFinal,
            OutputProcessed = isFinal
        };

        this.GetDataSerializer(currentStateDefinition.StateType).Restore(source, stateInstance);

        return stateInstance;
    }

    public void Serialize(StateInstance stateInstance)
    {
        var source = (TSource)stateInstance.Workflow.Source;

        this.GetDataSerializer(stateInstance.Definition.StateType).Store(source, stateInstance);
    }

    private IStateDataSerializer<TSource> GetDataSerializer(Type stateType)
    {
        var custom = serviceProvider.GetService(typeof(IStateDataSerializer<,>).MakeGenericType(typeof(TSource), stateType));

        if (custom is not null)
        {
            return (IStateDataSerializer<TSource>)custom;
        }
        else if (stateType == typeof(TaskState))
        {
            return new TaskStateDataSerializer<TSource>();
        }
        else
        {
            return EmptyStateDataSerializer.Instance;
        }
    }

    private sealed class EmptyStateDataSerializer : IStateDataSerializer<TSource>
    {
        public static readonly EmptyStateDataSerializer Instance = new();

        public void Restore(TSource source, StateInstance stateInstance)
        {
        }

        public void Store(TSource source, StateInstance stateInstance)
        {
        }
    }
}
