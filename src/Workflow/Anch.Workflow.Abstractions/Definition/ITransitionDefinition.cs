namespace Anch.Workflow.Definition;

public interface ITransitionDefinition
{
    IEventDefinition Event { get; }

    IStateDefinition To { get; }
}