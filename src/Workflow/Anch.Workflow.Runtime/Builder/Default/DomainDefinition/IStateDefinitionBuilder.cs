using Anch.Workflow.Domain.Definition;


namespace Anch.Workflow.Builder.Default.DomainDefinition;

public interface IStateDefinitionBuilder<TSource, TStatus> : IStateDefinition<TSource, TStatus>, IStateDefinitionBuilder
    where TSource : notnull
    where TStatus : struct
{
    new string Name { get; set; }

    bool IsAutoName { get; set; }

    List<EventDefinitionBuilder> Events { get; set; }

    new WorkflowDefinitionBuilder<TSource, TStatus> Workflow { get; set; }

    new List<TransitionDefinitionBuilder<TSource, TStatus>> Transitions { get; set; }
}

public interface IStateDefinitionBuilder : IStateDefinition
{
    new IReadOnlyList<IWorkflowDefinitionBuilder> SubWorkflows { get; }

    new IWorkflowDefinitionBuilder Workflow { get; }
}