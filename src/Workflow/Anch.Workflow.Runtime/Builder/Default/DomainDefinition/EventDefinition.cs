using Anch.Workflow.Definition;

namespace Anch.Workflow.Builder.Default.DomainDefinition
{
    public class EventDefinition : IEventDefinition
    {
        public EventHeader Header { get; set; } = null!;
    }
}