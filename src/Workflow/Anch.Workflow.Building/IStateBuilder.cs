using System.Linq.Expressions;

using Anch.Workflow.Building.Default.DomainDefinition;
using Anch.Workflow.States;

namespace Anch.Workflow.Building;

public interface IStateBuilder<TSource, TStatus>
    where TSource : class
    where TStatus : struct
{
    IStateDefinitionBuilder<TSource, TStatus> StateDefinitionBuilder { get; }
}

public interface IStateBuilder<TSource, TStatus, TState> : IWorkflowBuilder<TSource, TStatus>, IStateBuilder<TSource, TStatus>
    where TSource : class
    where TStatus : struct
    where TState : IState
{
    new StateDefinitionBuilder<TSource, TStatus, TState> StateDefinitionBuilder { get; }

    IStateBuilder<TSource, TStatus, TState> Input<TStateProperty, TSourceProperty>(Expression<Func<TState, TStateProperty>> getStateProperty, TSourceProperty value)
        where TSourceProperty : TStateProperty =>

        this.Input(getStateProperty, _ => value);

    IStateBuilder<TSource, TStatus, TState> Input<TStateProperty, TSourceProperty>(Expression<Func<TState, TStateProperty>> getStateProperty, Func<TSource, TSourceProperty> getSourceProperty)
        where TSourceProperty : TStateProperty =>

        this.Input(getStateProperty, async (TSource source, IWorkflowHost _, CancellationToken _) => getSourceProperty(source));

    IStateBuilder<TSource, TStatus, TState> Input<TStateProperty, TSourceProperty, TService>(
        Expression<Func<TState, TStateProperty>> getStateProperty,
        Func<TSource, TService, CancellationToken, ValueTask<TSourceProperty>> getSourceProperty)
        where TSourceProperty : TStateProperty
        where TService : notnull;

    IStateBuilder<TSource, TStatus, TState> Output<TSourceProperty, TStateProperty>(Expression<Func<TSource, TSourceProperty>> propertyValueSelector, TStateProperty value)
        where TStateProperty : TSourceProperty =>

        this.Output(propertyValueSelector, _ => value);

    IStateBuilder<TSource, TStatus, TState> Output<TSourceProperty, TStateProperty>(Expression<Func<TSource, TSourceProperty>> propertyValueSelector, Func<TState, TStateProperty> getStateProperty)
        where TStateProperty : TSourceProperty =>

        this.Output(propertyValueSelector, async (TState state, IWorkflowHost _, CancellationToken _) => getStateProperty(state));

    IStateBuilder<TSource, TStatus, TState> Output<TSourceProperty, TStateProperty, TService>(
        Expression<Func<TSource, TSourceProperty>> propertyValueSelector,
        Func<TState, TService, CancellationToken, ValueTask<TStateProperty>> getStateProperty)
        where TStateProperty : TSourceProperty
        where TService : notnull;

    IStateBuilder<TSource, TStatus, TState> WithName(string name);

    IStateBuilder<TSource, TStatus, TState> WithStatus(TStatus status);
}