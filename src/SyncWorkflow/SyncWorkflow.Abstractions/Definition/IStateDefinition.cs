using System.Collections.Frozen;
using System.Collections.Immutable;

namespace SyncWorkflow.Definition;

public interface IStateDefinition
{
    string Name { get; }

    object? Status { get; }

    IWorkflowDefinition Workflow { get; }

    Type StateType { get; set; }

    ImmutableArray<Delegate> InputActions { get; }

    ImmutableArray<Delegate> OutputActions { get; }

    ImmutableArray<ITransitionDefinition> Transitions { get; }

    ImmutableArray<IWorkflow> SubWorkflow { get; }

    FrozenDictionary<string, object> AdditionalInfo { get; }
}