using System.Linq.Expressions;

using SyncWorkflow.Builder.Default.DomainDefinition;
using SyncWorkflow.Engine;
using SyncWorkflow.States;
using SyncWorkflow.States._Base;

namespace SyncWorkflow.Builder;

public interface IStateBuilder
{
    StateDefinition StateDefinition { get; }
}

public interface IStateBuilder<TSource, TState> : IWorkflowBuilder<TSource>, IStateBuilder
    where TSource : notnull
    where TState : IState
{
    IStateBuilder<TSource, TState> Input<TStateProperty, TSourceProperty>(Expression<Func<TState, TStateProperty>> getStateProperty, TSourceProperty value)
        where TSourceProperty : TStateProperty
    {
        return this.Input(getStateProperty, _ => value);
    }

    IStateBuilder<TSource, TState> Input<TStateProperty, TSourceProperty>(Expression<Func<TState, TStateProperty>> getStateProperty, Func<TSource, TSourceProperty> getSourceProperty)
        where TSourceProperty : TStateProperty
    {
        return this.Input(getStateProperty, async (TSource source, IWorkflowHost _, CancellationToken _) => getSourceProperty(source));
    }

    IStateBuilder<TSource, TState> Input<TStateProperty, TSourceProperty, TService>(
        Expression<Func<TState, TStateProperty>> getStateProperty,
        Func<TSource, TService, CancellationToken, Task<TSourceProperty>> getSourceProperty)
        where TSourceProperty : TStateProperty
        where TService : notnull;

    IStateBuilder<TSource, TState> Output<TSourceProperty, TStateProperty>(Expression<Func<TSource, TSourceProperty>> propertyValueSelector, TStateProperty value)
        where TStateProperty : TSourceProperty
    {
        return this.Output(propertyValueSelector, _ => value);
    }

    IStateBuilder<TSource, TState> Output<TSourceProperty, TStateProperty>(Expression<Func<TSource, TSourceProperty>> propertyValueSelector, Func<TState, TStateProperty> getStateProperty)
        where TStateProperty : TSourceProperty
    {
        return this.Output(propertyValueSelector, async (TState state, IWorkflowHost _, CancellationToken _) => getStateProperty(state));
    }

    IStateBuilder<TSource, TState> Output<TSourceProperty, TStateProperty, TService>(
        Expression<Func<TSource, TSourceProperty>> propertyValueSelector,
        Func<TState, TService, CancellationToken, Task<TStateProperty>> getStateProperty)
        where TStateProperty : TSourceProperty
        where TService : notnull;

    IStateBuilder<TSource, TState> WithName(string name);

    IStateBuilder<TSource, TState> WithStatus(Enum status);
}