using Anch.Core;

namespace Anch.Workflow.Domain.Definition;

public interface IWorkflowDefinition<TSource, TStatus> : IWorkflowDefinition<TSource>
    where TSource : notnull
    where TStatus : struct
{
    PropertyAccessors<TSource, TStatus>? StatusAccessors { get; }

    new IReadOnlyList<IStateDefinition<TSource, TStatus>> States { get; }

    new IStateDefinition<TSource, TStatus> StartState { get; }

    new IStateDefinition<TSource, TStatus> DefaultFinalState { get; }

    new IStateDefinition<TSource, TStatus> TerminateState { get; }

    Type IWorkflowDefinition.StatusType => typeof(TStatus);
}

public interface IWorkflowDefinition<TSource> : IWorkflowDefinition
    where TSource : notnull
{
    PropertyAccessors<TSource, long>? VersionAccessors { get; }

    Type IWorkflowDefinition.SourceType => typeof(TSource);
}

public interface IWorkflowDefinition
{
    WorkflowDefinitionIdentity Identity { get; }

    Type SourceType { get; }

    Type StatusType { get; }

    long Version => 1;

    bool IsRoot { get; }

    IReadOnlyList<IStateDefinition> States { get; }

    IStateDefinition StartState { get; }

    IStateDefinition DefaultFinalState { get; }

    IStateDefinition TerminateState { get; }

    IReadOnlyDictionary<string, object> Settings { get; }
}