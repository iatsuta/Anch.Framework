using SyncWorkflow.Domain.Definition;

namespace SyncWorkflow.Builder.Default.DomainDefinition
{
    public class EventDefinition : IEventDefinition
    {
        public EventHeader Header { get; set; } = null!;
    }
}