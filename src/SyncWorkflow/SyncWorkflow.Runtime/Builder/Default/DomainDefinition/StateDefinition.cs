using SyncWorkflow.Domain.Definition;

namespace SyncWorkflow.Builder.Default.DomainDefinition;

public class StateDefinition : IStateDefinition
{
    public string Name { get; set; } = "";

    public object? Status { get; set; }

    public bool IsAutoName { get; set; } = true;

    public WorkflowDefinition Workflow { get; set; } = null!;

    public Type StateType { get; set; } = null!;

    public List<EventDefinition> Events { get; set; } = [];

    public List<Delegate> InputActions { get; set; } = [];

    public List<Delegate> OutputActions { get; set; } = [];

    public List<TransitionDefinition> Transitions { get; set; } = [];

    public List<BuildWorkflow> SubWorkflows { get; set; } = [];

    public Dictionary<string, object> AdditionalInfo { get; set; } = new();


    IReadOnlyDictionary<string, object> IStateDefinition.AdditionalInfo => this.AdditionalInfo;

    IWorkflowDefinition IStateDefinition.Workflow => this.Workflow;

    IEnumerable<Delegate> IStateDefinition.InputActions => this.InputActions;

    IEnumerable<Delegate> IStateDefinition.OutputActions => this.OutputActions;

    IEnumerable<ITransitionDefinition> IStateDefinition.Transitions => this.Transitions;

    IEnumerable<IWorkflow> IStateDefinition.SubWorkflow => this.SubWorkflows;


    public override string ToString()
    {
        return $"{this.Name} ({this.StateType.Name})";
    }


    public const string SystemEmptyName = "SystemEmpty";

    public const string SystemFinalName = "DefaultFinal";

    public const string SystemTerminateName = "Terminate";
}