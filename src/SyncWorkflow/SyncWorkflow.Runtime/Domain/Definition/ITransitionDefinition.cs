namespace SyncWorkflow.Domain.Definition;

public interface ITransitionDefinition
{
    public IEventDefinition Event { get; }

    public IStateDefinition To { get; }
}