namespace SyncWorkflow.Domain.Definition;

public interface IStateDefinition
{
    string Name { get; }

    object? Status { get; }

    IWorkflowDefinition Workflow { get; }

    Type StateType { get; set; }

    IEnumerable<Delegate> InputActions { get; }

    IEnumerable<Delegate> OutputActions { get; }

    IEnumerable<ITransitionDefinition> Transitions { get; }

    IEnumerable<IWorkflow> SubWorkflow { get; }

    IReadOnlyDictionary<string, object> AdditionalInfo { get; }
}