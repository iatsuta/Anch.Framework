using System.Collections.Frozen;
using System.Collections.Immutable;

using Anch.Workflow.Definition;

namespace Anch.Workflow.Builder.Default.DomainDefinition;

public class StateDefinition : IStateDefinition
{
    public string Name { get; set; } = "";

    public object? Status { get; set; }

    public bool IsAutoName { get; set; } = true;

    public WorkflowDefinition Workflow { get; set; } = null!;

    public Type StateType { get; set; } = null!;

    public List<EventDefinition> Events { get; } = [];

    public List<Delegate> InputActions { get; } = [];

    public List<Delegate> OutputActions { get; } = [];

    public List<TransitionDefinition> Transitions { get; set; } = [];

    public List<BuildWorkflow> SubWorkflows { get; set; } = [];

    public Dictionary<string, object> AdditionalInfo { get; set; } = [];


    FrozenDictionary<string, object> IStateDefinition.AdditionalInfo => field ??= this.AdditionalInfo.ToFrozenDictionary();

    IWorkflowDefinition IStateDefinition.Workflow => this.Workflow;

    ImmutableList<Delegate> IStateDefinition.InputActions => field ??= [.. this.InputActions];

    ImmutableList<Delegate> IStateDefinition.OutputActions => field ??= [.. this.OutputActions];

    ImmutableList<ITransitionDefinition> IStateDefinition.Transitions => field ??= this.Transitions.ToImmutableList<ITransitionDefinition>();

    ImmutableList<IWorkflow> IStateDefinition.SubWorkflow => field ??= this.SubWorkflows.ToImmutableList<IWorkflow>();


    public override string ToString()
    {
        return $"{this.Name} ({this.StateType.Name})";
    }


    public const string SystemEmptyName = "SystemEmpty";

    public const string SystemFinalName = "DefaultFinal";

    public const string SystemTerminateName = "Terminate";
}