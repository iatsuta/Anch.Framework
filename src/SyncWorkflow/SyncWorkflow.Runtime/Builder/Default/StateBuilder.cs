using System.Linq.Expressions;
using CommonFramework;
using Microsoft.Extensions.DependencyInjection;
using SyncWorkflow.Builder.Default.DomainDefinition;
using SyncWorkflow.States;
using SyncWorkflow.States._Base;

namespace SyncWorkflow.Builder.Default;

public class StateBuilder<TSource, TState> : WorkflowBuilder<TSource>, IStateBuilder<TSource, TState>
    where TState : IState where TSource : notnull
{
    public StateBuilder(WorkflowDefinition workflow, bool addDoneEvent)
        : base(workflow)
    {
        this.StateDefinition.StateType = typeof(TState);
        this.StateDefinition.Workflow = this.Workflow;

        if (addDoneEvent)
        {
            this.StateDefinition.Events.Add(new EventDefinition
            {
                Header = EventHeader.StateDone
            });
        }

        this.Workflow.AddState(this.StateDefinition);
    }

    public StateDefinition StateDefinition { get; } = new();

    public IStateBuilder<TSource, TState> Input<TStateProperty, TSourceProperty, TService>(
        Expression<Func<TState, TStateProperty>> getStateProperty,
        Func<TSource, TService, CancellationToken, Task<TSourceProperty>> getSourceProperty)
        where TSourceProperty : TStateProperty
        where TService : notnull

    {
        var setStatePropertyAction = getStateProperty.GetProperty().GetSetValueAction<TState, TStateProperty>();

        Func<IServiceProvider, TSource, TState, CancellationToken, Task> setAction = async (serviceProvider, source, state, ct) =>

            setStatePropertyAction(state, await getSourceProperty(source, serviceProvider.GetRequiredService<TService>(), ct));

        this.StateDefinition.InputActions.Add(setAction);

        return this;
    }

    public IStateBuilder<TSource, TState> Output<TSourceProperty, TStateProperty, TService>(
        Expression<Func<TSource, TSourceProperty>> getSourceProperty,
        Func<TState, TService, CancellationToken, Task<TStateProperty>> getStateProperty)
        where TStateProperty : TSourceProperty
        where TService : notnull
    {
        var setSourcePropertyAction = getSourceProperty.GetProperty().GetSetValueAction<TSource, TSourceProperty>();

        Func<IServiceProvider, TState, TSource, CancellationToken,  Task> setAction = async (serviceProvider, state, source, ct) =>

            setSourcePropertyAction(source, await getStateProperty(state, serviceProvider.GetRequiredService<TService>(), ct));

        this.StateDefinition.OutputActions.Add(setAction);

        return this;
    }

    public IStateBuilder<TSource, TState> WithName(string name)
    {
        this.StateDefinition.Name = name;
        this.StateDefinition.IsAutoName = false;

        return this;
    }

    public IStateBuilder<TSource, TState> WithStatus(Enum status)
    {
        this.StateDefinition.Status = status;

        return this.WithName(status.ToString());
    }
}