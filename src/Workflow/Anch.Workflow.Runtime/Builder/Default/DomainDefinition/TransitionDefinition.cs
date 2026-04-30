using Anch.Workflow.Definition;

namespace Anch.Workflow.Builder.Default.DomainDefinition;

public class TransitionDefinition : ITransitionDefinition
{
    public EventDefinition Event { get; set; } = null!;

    public StateDefinition To { get; set; } = null!;


    IEventDefinition ITransitionDefinition.Event => this.Event;

    IStateDefinition ITransitionDefinition.To => this.To;
}