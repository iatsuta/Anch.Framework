using System.Linq.Expressions;

using Anch.Core;
using Anch.Workflow.Domain.Definition;
using Anch.Workflow.States;

namespace Anch.Workflow.Building;

public interface IWorkflowBuilder<TSource, TStatus>
    where TSource : class
    where TStatus : struct
{
    IWorkflowBuilder<TSource, TStatus> WithIdentity(string name)
    {
        return this.WithIdentity(new WorkflowDefinitionIdentity(name));
    }

    IWorkflowBuilder<TSource, TStatus> WithStatusProperty(Expression<Func<TSource, TStatus>> statusPath);

    IWorkflowBuilder<TSource, TStatus> WithVersionProperty(Expression<Func<TSource, long>> versionPath);

    IWorkflowBuilder<TSource, TStatus> WithSetting(string name, object value);

    IWorkflowBuilder<TSource, TStatus> WithIdentity(WorkflowDefinitionIdentity identity);

    IStateBuilder<TSource, TStatus, TState> Then<TState>()
        where TState : IState;

    IStateBuilder<TSource, TStatus, ActionState<TSource, IServiceProvider>> Then(Action<TSource> action)
    {
        return this.Then<IServiceProvider>(async (source, _, _) => action(source));
    }

    IStateBuilder<TSource, TStatus, ActionState<TSource, TService>> Then<TService>(Func<TSource, TService, CancellationToken, ValueTask> action)
        where TService : notnull
    {
        return this.Then<ActionState<TSource, TService>>()
            .Input(s => s.Action, action);
    }

    IWorkflowBuilder<TSource, TStatus> Then(IStateBuilder<TSource, TStatus> state);

    IStateBuilder<TSource, TStatus, StartWorkflowState<TInnerSource>> StartWorkflow<TInnerSource, TInnerWorkflow>(Func<TSource, TInnerSource> getInnerSource)
        where TInnerWorkflow : IWorkflow<TInnerSource>
        where TInnerSource : class;

    IStateBuilder<TSource, TStatus, StartWorkflowsState<TSource, TInnerSource>> StartWorkflows<TInnerSource, TInnerWorkflow>(
        Func<TSource, IEnumerable<TInnerSource>> getElements)
        where TInnerWorkflow : IWorkflow<TInnerSource>
        where TInnerSource : class;

    IStateBuilder<TSource, TStatus, IfState> If(
        Func<TSource, bool> condition,
        Action<IWorkflowBuilder<TSource, TStatus>> trueSetupWorkflowBuilder,
        Action<IWorkflowBuilder<TSource, TStatus>>? falseSetupWorkflowBuilder = null) =>

        this.If<IServiceProvider>((source, _) => condition(source), trueSetupWorkflowBuilder, falseSetupWorkflowBuilder);

    IStateBuilder<TSource, TStatus, IfState> If<TService>(
        Func<TSource, TService, bool> condition,
        Action<IWorkflowBuilder<TSource, TStatus>> trueSetupWorkflowBuilder,
        Action<IWorkflowBuilder<TSource, TStatus>>? falseSetupWorkflowBuilder = null)
        where TService : notnull =>

        this.If<TService>(async (source, service, _) => condition(source, service), trueSetupWorkflowBuilder, falseSetupWorkflowBuilder);

    IStateBuilder<TSource, TStatus, IfState> If<TService>(
        Func<TSource, TService, CancellationToken, ValueTask<bool>> condition,
        Action<IWorkflowBuilder<TSource, TStatus>> trueSetupWorkflowBuilder,
        Action<IWorkflowBuilder<TSource, TStatus>>? falseSetupWorkflowBuilder = null)
        where TService : notnull;

    IStateBuilder<TSource, TStatus, SwitchState<TProperty>> Switch<TProperty>(Func<TSource, TProperty> selector,

        params (TProperty CaseValue, Action<IWorkflowBuilder<TSource, TStatus>> CaseSetupWorkflowBuilder)[] cases)
        where TProperty : notnull
    {
        return this.Switch(selector, _ => { }, cases);
    }

    IStateBuilder<TSource, TStatus, SwitchState<TProperty>> Switch<TProperty>(
        Func<TSource, TProperty> selector,

        Action<IWorkflowBuilder<TSource, TStatus>> defaultCaseSetupWorkflowBuilder,

        params (TProperty CaseValue, Action<IWorkflowBuilder<TSource, TStatus>> CaseSetupWorkflowBuilder)[] cases)
        where TProperty : notnull
    {
        return this.Switch<TProperty, IServiceProvider>(async (source, _, _) => selector(source), defaultCaseSetupWorkflowBuilder, cases);
    }

    IStateBuilder<TSource, TStatus, SwitchState<TProperty>> Switch<TProperty, TService>(
        Func<TSource, TService, CancellationToken, ValueTask<TProperty>> selector,
        params (TProperty CaseValue, Action<IWorkflowBuilder<TSource, TStatus>> CaseSetupWorkflowBuilder)[] cases)
        where TService : notnull
        where TProperty : notnull
    {
        return this.Switch(selector, _ => { }, cases);
    }

    IStateBuilder<TSource, TStatus, SwitchState<TProperty>> Switch<TProperty, TService>(
        Func<TSource, TService, CancellationToken, ValueTask<TProperty>> selector,
        Action<IWorkflowBuilder<TSource, TStatus>> defaultCaseSetupWorkflowBuilder,
        params (TProperty CaseValue, Action<IWorkflowBuilder<TSource, TStatus>> CaseSetupWorkflowBuilder)[] cases)
        where TService : notnull
        where TProperty : notnull;


    IStateBuilder<TSource, TStatus, ParallelForeachState<TSource, TElement>> ParallelForeach<TElement>(
        Func<TSource, IEnumerable<TElement>> getElements,
        Action<IWorkflowBuilder<SourceItem<TSource, TElement>, Ignore>> setupIteratorBuilder)
    {
        return this.ParallelForeach<TElement, IServiceProvider>((source, _) => getElements(source).ToAsyncEnumerable(), setupIteratorBuilder);
    }

    public IStateBuilder<TSource, TStatus, ParallelForeachState<TSource, TElement>> ParallelForeach<TElement, TService>(
        Func<TSource, TService, IAsyncEnumerable<TElement>> getElements,
        Action<IWorkflowBuilder<SourceItem<TSource, TElement>, Ignore>> setupIteratorBuilder)
        where TService : notnull;

    IStateBuilder<TSource, TStatus, ParallelState<TSource>> Parallel(params Action<IWorkflowBuilder<TSource, TStatus>>[] setupForks);

    IStateBuilder<TSource, TStatus, ForeachState<TSource, TElement>> Foreach<TElement>(
        Func<TSource, IEnumerable<TElement>> getElements,
        Action<IWorkflowBuilder<SourceItem<TSource, TElement>, Ignore>> setupIteratorBuilder)
    {
        return this.Foreach<TElement, IServiceProvider>((source, _) => getElements(source).ToAsyncEnumerable(), setupIteratorBuilder);
    }

    IStateBuilder<TSource, TStatus, ForeachState<TSource, TElement>> Foreach<TElement, TService>(
        Func<TSource, TService, IAsyncEnumerable<TElement>> getElements,
        Action<IWorkflowBuilder<SourceItem<TSource, TElement>, Ignore>> setupIteratorBuilder)
        where TService : notnull;

    IStateBuilder<TSource, TStatus, FinalState> Finish(object? result = null) => this.Finish(_ => result);

    IStateBuilder<TSource, TStatus, FinalState> Finish(Func<TSource, object?> getResult);

    IStateBuilder<TSource, TStatus, TaskState> Task(Action<ITaskBuilder<TSource, TStatus>> setup);
}