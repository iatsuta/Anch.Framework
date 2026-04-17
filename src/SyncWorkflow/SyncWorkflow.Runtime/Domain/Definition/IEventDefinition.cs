namespace SyncWorkflow.Domain.Definition;

public interface IEventDefinition
{
    EventHeader Header { get; }
}