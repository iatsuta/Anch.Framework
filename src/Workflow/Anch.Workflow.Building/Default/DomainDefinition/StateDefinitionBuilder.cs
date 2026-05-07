using Anch.Workflow.Domain.Definition;

namespace Anch.Workflow.Building.Default.DomainDefinition;

public class StateDefinitionBuilder<TSource, TStatus, TState> : IStateDefinitionBuilder<TSource, TStatus>, IStateDefinition<TSource, TStatus, TState>
    where TSource : class
    where TStatus : struct
{
    public string Name { get; set; } = "";

    public TStatus? Status { get; set; } = null;

    public bool IsAutoName { get; set; } = true;

    public WorkflowDefinitionBuilder<TSource, TStatus> Workflow { get; set; } = null!;

    public List<EventDefinitionBuilder> Events { get; set; } = [];

    public List<Func<IServiceProvider, TSource, TState, CancellationToken, ValueTask>> InputActions { get; set; } = [];

    public List<Func<IServiceProvider, TState, TSource, CancellationToken, ValueTask>> OutputActions { get; set; } = [];

    public List<TransitionDefinitionBuilder<TSource, TStatus>> Transitions { get; set; } = [];

    public List<Lazy<IWorkflowDefinitionBuilder>> LazySubWorkflows { get; set; } = [];

    public IReadOnlyList<IWorkflowDefinitionBuilder> SubWorkflows => field ??= [.. this.LazySubWorkflows.Select(v => v.Value)];

    public Dictionary<string, object> AdditionalInfo { get; set; } = [];

    public override string ToString()
    {
        return $"{this.Name} ({((IStateDefinition)this).StateType.Name})";
    }


    IReadOnlyDictionary<string, object> IStateDefinition.AdditionalInfo => field ??= this.AdditionalInfo;

    IWorkflowDefinitionBuilder IStateDefinitionBuilder.Workflow => this.Workflow;

    IWorkflowDefinition<TSource, TStatus> IStateDefinition<TSource, TStatus>.Workflow => this.Workflow;

    IReadOnlyList<ITransitionDefinition<TSource, TStatus>> IStateDefinition<TSource, TStatus>.Transitions => this.Transitions;


    IWorkflowDefinition IStateDefinition.Workflow => this.Workflow;

    IReadOnlyList<ITransitionDefinition> IStateDefinition.Transitions => this.Transitions;


    IReadOnlyList<IWorkflowDefinition> IStateDefinition.SubWorkflows => this.SubWorkflows;

    IReadOnlyList<Func<IServiceProvider, TSource, TState, CancellationToken, ValueTask>> IStateDefinition<TSource, TStatus, TState>.InputActions => this.InputActions;

    IReadOnlyList<Func<IServiceProvider, TState, TSource, CancellationToken, ValueTask>> IStateDefinition<TSource, TStatus, TState>.OutputActions => this.OutputActions;

}

public static class StateDefinitionBuilder
{
    public const string SystemEmptyName = "SystemEmpty";

    public const string SystemFinalName = "DefaultFinal";

    public const string SystemTerminateName = "Terminate";
}