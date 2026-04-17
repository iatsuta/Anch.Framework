using SyncWorkflow.Builder.Default;
using SyncWorkflow.States;
using SyncWorkflow.States._Base;

namespace SyncWorkflow.Builder;

public static class StateBuilderExtensions
{
    public static IStateBuilder<TSource, TState> SetBreak<TSource, TState>(
        this IStateBuilder<TSource, TState> stateBuilder,
        StateBreakPolicy breakPolicy)
        where TSource : notnull
        where TState : ParallelStateBase<TSource>
    {
        return stateBuilder
            .Input(state => state.BreakPolicy, breakPolicy);
    }

    public static IStateBuilder<TSource, ParallelForeachState<TSource, TElement>> SetFinishedBreak<TSource, TElement>(
        this IStateBuilder<TSource, ParallelForeachState<TSource, TElement>> stateBuilder,
        Func<TElement, bool> condition)
        where TSource : notnull
    {
        return stateBuilder
            .SetBreak(StateBreakPolicy.AnyFinishedItem<(TSource Source, TElement Element)>(pair => condition(pair.Element)));
    }

    public static IStateBuilder<TSource, StartWorkflowsState<TSource, TElement>> SetFinishedBreak<TSource, TElement>(
        this IStateBuilder<TSource, StartWorkflowsState<TSource, TElement>> stateBuilder,
        Func<TElement, bool> condition)
        where TSource : notnull
        where TElement : notnull
    {
        return stateBuilder
            .SetBreak(StateBreakPolicy.AnyFinishedItem(condition));
    }

    internal static IStateBuilder<TSource, TState> WithSubWorkflow<TSource, TState>(this IStateBuilder<TSource, TState> stateBuilder, IEnumerable<BuildWorkflow> subWorkflows)
        where TSource : notnull
        where TState : IState
    {
        stateBuilder.StateDefinition.SubWorkflows.AddRange(subWorkflows);

        return stateBuilder;
    }
}