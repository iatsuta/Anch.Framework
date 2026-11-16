using Anch.Core;
using Anch.Workflow.Domain;
using Anch.Workflow.Domain.Definition;
using Anch.Workflow.Domain.Runtime;
using Anch.Workflow.States;

namespace Anch.Workflow.Persistence.Inline;

public class StateInstanceSerializer<TSource, TStatus>(
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

        var isFinal = new[] { typeof(FinalState), typeof(TerminateState) }.Contains(currentStateDefinition.StateType);

        var stateInstance = new StateInstance
        {
            Id = workflowInstance.Id,
            Workflow = workflowInstance,
            Definition = currentStateDefinition,
            InputProcessed = isFinal,
            OutputProcessed = isFinal
        };

        stateInstance.WaitEvents.AddRange(this.GetWaitEvents(stateInstance));

        return stateInstance;
    }

    private IEnumerable<WaitEventInfo> GetWaitEvents(StateInstance stateInstance)
    {
        if (stateInstance.Definition.StateType == typeof(TaskState))
        {
            throw new NotImplementedException();
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    public void Serialize(StateInstance workflowInstance)
    {
        throw new NotImplementedException();
    }
}