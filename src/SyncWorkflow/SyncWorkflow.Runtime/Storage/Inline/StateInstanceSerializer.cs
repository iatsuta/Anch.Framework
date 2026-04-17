using SyncWorkflow.Domain.Runtime;
using SyncWorkflow.States;

namespace SyncWorkflow.Storage.Inline;

public class StateInstanceSerializer<TSource>(IWorkflow<TSource> workflow,
    IStateDefinitionResolverFactory stateDefinitionResolverFactory) : IStateInstanceSerializer<TSource>
{
    private readonly IStateDefinitionResolver<TSource> stateDefinitionResolver = stateDefinitionResolverFactory.Create(workflow);

    public StateInstance Deserialize(TSource source)
    {
        var currentStateDefinition = this.stateDefinitionResolver.GetCurrentStateDefinition(source);

        var isFinished = currentStateDefinition.StateType == typeof(FinalState);
        var isTerminated = currentStateDefinition.StateType == typeof(TerminateState);

        var stateInstance = new StateInstance
        {
            Id = source.Id,
            Workflow = result,
            Definition = currentStateDefinition,
            InputProcessed = true,
            OutputProcessed = isFinished || isTerminated
        };
    }

    public void Serialize(StateInstance workflowInstance)
    {
        throw new NotImplementedException();
    }
}