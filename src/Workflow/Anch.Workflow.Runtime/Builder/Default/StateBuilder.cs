using System.Linq.Expressions;

using Anch.Core;
using Anch.Workflow.Builder.Default.DomainDefinition;
using Anch.Workflow.Domain;
using Anch.Workflow.States;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.Workflow.Builder.Default;

public class StateBuilder<TSource, TStatus, TState> : WorkflowBuilder<TSource, TStatus>, IStateBuilder<TSource, TStatus, TState>
    where TSource : notnull
    where TStatus : struct
    where TState : IState
{
    public StateBuilder(WorkflowDefinitionBuilder<TSource, TStatus> workflowDefinitionBuilder, bool addDoneEvent)
        : base(workflowDefinitionBuilder)
    {
        this.StateDefinitionBuilder.Workflow = workflowDefinitionBuilder;

        if (addDoneEvent)
        {
            this.StateDefinitionBuilder.Events.Add(new EventDefinitionBuilder
            {
                Header = EventHeader.StateDone
            });
        }

        workflowDefinitionBuilder.AddState(this.StateDefinitionBuilder);
    }

    public StateDefinitionBuilder<TSource, TStatus, TState> StateDefinitionBuilder { get; } = new();

    public IStateBuilder<TSource, TStatus, TState> Input<TStateProperty, TSourceProperty, TService>(
        Expression<Func<TState, TStateProperty>> getStateProperty,
        Func<TSource, TService, CancellationToken, ValueTask<TSourceProperty>> getSourceProperty)
        where TSourceProperty : TStateProperty
        where TService : notnull

    {
        var statePropertySetter = getStateProperty.ToPropertyAccessors().Setter;

        this.StateDefinitionBuilder.InputActions.Add(async (serviceProvider, source, state, ct) =>

            statePropertySetter(state, await getSourceProperty(source, serviceProvider.GetRequiredService<TService>(), ct)));

        return this;
    }

    public IStateBuilder<TSource, TStatus, TState> Output<TSourceProperty, TStateProperty, TService>(
        Expression<Func<TSource, TSourceProperty>> propertyValueSelector,
        Func<TState, TService, CancellationToken, ValueTask<TStateProperty>> getStateProperty)
        where TStateProperty : TSourceProperty
        where TService : notnull
    {
        var propertyValueSetter = propertyValueSelector.ToPropertyAccessors().Setter;

        this.StateDefinitionBuilder.OutputActions.Add(async (serviceProvider, state, source, ct) =>

            propertyValueSetter(source, await getStateProperty(state, serviceProvider.GetRequiredService<TService>(), ct)));

        return this;
    }

    public IStateBuilder<TSource, TStatus, TState> WithName(string name)
    {
        this.StateDefinitionBuilder.Name = name;
        this.StateDefinitionBuilder.IsAutoName = false;

        return this;
    }

    public IStateBuilder<TSource, TStatus, TState> WithStatus(TStatus status)
    {
        this.StateDefinitionBuilder.Status = status;

        if (this.StateDefinitionBuilder.IsAutoName && status.ToString() is { } name)
        {
            return this.WithName(name);
        }
        else
        {
            return this;
        }
    }

    IStateDefinitionBuilder<TSource, TStatus> IStateBuilder<TSource, TStatus>.StateDefinitionBuilder => this.StateDefinitionBuilder;
}