using Anch.Workflow.Domain;
using Anch.Workflow.Domain.Definition;

namespace Anch.Workflow.Building.Default.DomainDefinition;

public class EventDefinitionBuilder : IEventDefinition
{
    public EventHeader Header { get; set; } = null!;
}