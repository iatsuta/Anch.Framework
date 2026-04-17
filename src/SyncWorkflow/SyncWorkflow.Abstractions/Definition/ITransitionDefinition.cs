namespace SyncWorkflow.Definition;

public interface ITransitionDefinition
{
    public IEventDefinition Event { get; }

    public IStateDefinition To { get; }
}