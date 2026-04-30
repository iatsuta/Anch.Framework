using System.Linq.Expressions;

using Anch.Workflow.Definition;
using Anch.Workflow.States;
using Anch.Workflow.States._Base;

namespace Anch.Workflow.Builder;

public interface IWorkflowBuilder<TSource>
    where TSource : notnull
{
    IWorkflowBuilder<TSource> WithIdentity(string name)
    {
        return this.WithIdentity(new WorkflowDefinitionIdentity(name));
    }

    IWorkflowBuilder<TSource> WithIdProperty(Expression<Func<TSource, Guid>> idPath);

    IWorkflowBuilder<TSource> WithStatusProperty<TStatus>(Expression<Func<TSource, TStatus>> statusPath);

    IWorkflowBuilder<TSource> WithVersionProperty<TStatus>(Expression<Func<TSource, Guid>> versionPath);

    IWorkflowBuilder<TSource> WithSetting(string name, object value);

    IWorkflowBuilder<TSource> WithIdentity(WorkflowDefinitionIdentity identity);

    IStateBuilder<TSource, TState> Then<TState>()
        where TState : IState;

    IStateBuilder<TSource, ActionState<TSource, IServiceProvider>> Then(Action<TSource> action)
    {
        return this.Then<IServiceProvider>(async (source, _, _) => action(source));
    }

    IStateBuilder<TSource, ActionState<TSource, TService>> Then<TService>(Func<TSource, TService, CancellationToken, Task> action)
        where TService : notnull
    {
        return this.Then<ActionState<TSource, TService>>()
            .Input(s => s.Action, action);
    }

    IWorkflowBuilder<TSource> Then(IStateBuilder state);

    IStateBuilder<TSource, StartWorkflowState<TInnerSource>> StartWorkflow<TInnerSource, TWorkflow>(Func<TSource, TInnerSource> getInnerSource)
        where TWorkflow : IWorkflow<TInnerSource>
        where TInnerSource : notnull;

    IStateBuilder<TSource, StartWorkflowsState<TSource, TInnerSource>> StartWorkflows<TInnerSource, TWorkflow>(Func<TSource, IEnumerable<TInnerSource>> getElements)
        where TWorkflow : IWorkflow<TInnerSource>;

    IStateBuilder<TSource, IfState> If(
        Func<TSource, bool> condition,
        Action<IWorkflowBuilder<TSource>> trueSetupWorkflowBuilder,
        Action<IWorkflowBuilder<TSource>>? falseSetupWorkflowBuilder = null)
    {
        return this.If<IServiceProvider>(async (source, _, _) => condition(source), trueSetupWorkflowBuilder, falseSetupWorkflowBuilder);
    }

    IStateBuilder<TSource, IfState> If<TService>(
       Func<TSource, TService, CancellationToken, Task<bool>> condition,
       Action<IWorkflowBuilder<TSource>> trueSetupWorkflowBuilder,
       Action<IWorkflowBuilder<TSource>>? falseSetupWorkflowBuilder = null)
        where TService : notnull;

    IStateBuilder<TSource, SwitchState<TProperty>> Switch<TProperty>(Func<TSource, TProperty> selector,

        params (TProperty CaseValue, Action<IWorkflowBuilder<TSource>> CaseSetupWorkflowBuilder)[] cases)
    {
        return this.Switch(selector, _ => {}, cases);
    }

    IStateBuilder<TSource, SwitchState<TProperty>> Switch<TProperty>(
        Func<TSource, TProperty> selector,

        Action<IWorkflowBuilder<TSource>> defaultCaseSetupWorkflowBuilder,

        params (TProperty CaseValue, Action<IWorkflowBuilder<TSource>> CaseSetupWorkflowBuilder)[] cases)
    {
        return this.Switch<TProperty, IServiceProvider>(async (source, _, _) => selector(source), defaultCaseSetupWorkflowBuilder, cases);
    }

    IStateBuilder<TSource, SwitchState<TProperty>> Switch<TProperty, TService>(
        Func<TSource, TService, CancellationToken, Task<TProperty>> selector,
        params (TProperty CaseValue, Action<IWorkflowBuilder<TSource>> CaseSetupWorkflowBuilder)[] cases)
        where TService : notnull
    {
        return this.Switch(selector, _ => { }, cases);
    }

    IStateBuilder<TSource, SwitchState<TProperty>> Switch<TProperty, TService>(
        Func<TSource, TService, CancellationToken, Task<TProperty>> selector,
        Action<IWorkflowBuilder<TSource>> defaultCaseSetupWorkflowBuilder,
        params (TProperty CaseValue, Action<IWorkflowBuilder<TSource>> CaseSetupWorkflowBuilder)[] cases)
        where TService : notnull;


    IStateBuilder<TSource, ParallelForeachState<TSource, TElement>> ParallelForeach<TElement>(
            Func<TSource, IEnumerable<TElement>> getElements,
            Action<IWorkflowBuilder<(TSource Source, TElement Element)>> setupIteratorBuilder)
    {
        return this.ParallelForeach<TElement, IServiceProvider>(async (source, _, _) => getElements(source), setupIteratorBuilder);
    }

    public IStateBuilder<TSource, ParallelForeachState<TSource, TElement>> ParallelForeach<TElement, TService>(
        Func<TSource, TService, CancellationToken, Task<IEnumerable<TElement>>> getElements,
        Action<IWorkflowBuilder<(TSource Source, TElement Element)>> setupIteratorBuilder)
        where TService : notnull;

    IStateBuilder<TSource, ParallelState<TSource>> Parallel(params Action<IWorkflowBuilder<TSource>>[] setupForks);

    IStateBuilder<TSource, ForeachState<TSource, TElement>> Foreach<TElement>(
        Func<TSource, IEnumerable<TElement>> getElements,
        Action<IWorkflowBuilder<(TSource Source, TElement Element)>> setupIteratorBuilder)
    {
        return this.Foreach<TElement, IServiceProvider>(async (source, _, _) => getElements(source), setupIteratorBuilder);
    }

    IStateBuilder<TSource, ForeachState<TSource, TElement>> Foreach<TElement, TService>(
        Func<TSource, TService, CancellationToken, Task<IEnumerable<TElement>>> getElements,
        Action<IWorkflowBuilder<(TSource Source, TElement Element)>> setupIteratorBuilder)
        where TService : notnull;
    

        IStateBuilder<TSource, FinalState> Finish(object? result = null)
    {
        return this.Finish(_ => result);
    }

    IStateBuilder<TSource, FinalState> Finish(Func<TSource, object?> getResult);

    IStateBuilder<TSource, TaskState> Task(Action<ITaskBuilder<TSource>> setup);
}