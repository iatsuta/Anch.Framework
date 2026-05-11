using Anch.Workflow.Building.Default.DomainDefinition;
using Anch.Workflow.States;

namespace Anch.Workflow.Building;

public static class StateBuilderExtensions
{
    public static IStateBuilder<TSource, TStatus, TState> SetBreak<TSource, TStatus, TState>(
        this IStateBuilder<TSource, TStatus, TState> stateBuilder,
        StateBreakPolicy breakPolicy)
        where TSource : class
        where TStatus : struct
        where TState : ParallelStateBase<TSource> =>

        stateBuilder.Input(state => state.BreakPolicy, breakPolicy);

    public static IStateBuilder<TSource, TStatus, ParallelForeachState<TSource, TElement>> SetFinishedBreak<TSource, TStatus, TElement>(
        this IStateBuilder<TSource, TStatus, ParallelForeachState<TSource, TElement>> stateBuilder,
        Func<TElement, bool> condition)
        where TSource : class
        where TStatus : struct =>

        stateBuilder.SetBreak(StateBreakPolicy.AnyFinishedItem<SourceItem<TSource, TElement>>(pair => condition(pair.Element)));

    public static IStateBuilder<TSource, TStatus, StartWorkflowsState<TSource, TElement>> SetFinishedBreak<TSource, TStatus, TElement>(
        this IStateBuilder<TSource, TStatus, StartWorkflowsState<TSource, TElement>> stateBuilder,
        Func<TElement, bool> condition)
        where TSource : class
        where TStatus : struct
        where TElement : class =>

        stateBuilder.SetBreak(StateBreakPolicy.AnyFinishedItem(condition));

    internal static IStateBuilder<TSource, TStatus, TState> WithSubWorkflow<TSource, TStatus, TState>(this IStateBuilder<TSource, TStatus, TState> stateBuilder, IEnumerable<Lazy<IWorkflowDefinitionBuilder>> subWorkflows)
        where TSource : class
        where TStatus : struct
        where TState : IState
    {
        stateBuilder.StateDefinitionBuilder.LazySubWorkflows.AddRange(subWorkflows);

        return stateBuilder;
    }
}