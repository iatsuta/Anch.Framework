using System.Linq.Expressions;

using Anch.Core;
using Anch.Workflow.Building.Default.DomainDefinition;
using Anch.Workflow.Domain;
using Anch.Workflow.Domain.Definition;
using Anch.Workflow.States;

namespace Anch.Workflow.Building.Default;

public class WorkflowBuilder<TSource, TStatus>(WorkflowDefinitionBuilder<TSource, TStatus> workflowDefinitionBuilder) : IWorkflowBuilder<TSource, TStatus>
    where TSource : class
    where TStatus : struct
{
    public IWorkflowBuilder<TSource, TStatus> WithStatusProperty(Expression<Func<TSource, TStatus>> statusPath)
    {
        workflowDefinitionBuilder.StatusAccessors = statusPath.ToPropertyAccessors();

        return this;
    }

    public IWorkflowBuilder<TSource, TStatus> WithVersionProperty(Expression<Func<TSource, long>> versionPath)
    {
        workflowDefinitionBuilder.VersionAccessors = versionPath.ToPropertyAccessors();

        return this;
    }

    public IWorkflowBuilder<TSource, TStatus> WithSetting(string name, object value)
    {
        workflowDefinitionBuilder.Settings[name] = value;

        return this;
    }

    public IWorkflowBuilder<TSource, TStatus> WithIdentity(WorkflowDefinitionIdentity identity)
    {
        workflowDefinitionBuilder.Identity = identity;
        workflowDefinitionBuilder.IsAutoIdentity = false;

        return this;
    }

    public IStateBuilder<TSource, TStatus, TState> Then<TState>()
        where TState : IState => this.ThenInternal<TState>(true);

    private StateBuilder<TSource, TStatus, TState> ThenInternal<TState>(bool addDoneEvent)

        where TState : IState => new(workflowDefinitionBuilder, addDoneEvent);

    public IWorkflowBuilder<TSource, TStatus> Then(IStateBuilder<TSource, TStatus> state)
    {
        workflowDefinitionBuilder.UpdateAutoFinish(state.StateDefinitionBuilder);

        return this;
    }

    public IStateBuilder<TSource, TStatus, StartWorkflowState<TInnerSource>> StartWorkflow<TInnerSource, TInnerWorkflow>(
        Func<TSource, TInnerSource> getInnerSource)
        where TInnerWorkflow : IWorkflow<TInnerSource>
        where TInnerSource : class
    {
        return this.Then<StartWorkflowState<TInnerSource>>()
            .Input(s => s.InnerWorkflow, async (TSource _, TInnerWorkflow innerWorkflow, CancellationToken _) => innerWorkflow.Definition)
            .Input(s => s.InnerSource, getInnerSource);
    }

    public IStateBuilder<TSource, TStatus, StartWorkflowsState<TSource, TInnerSource>> StartWorkflows<TInnerSource, TInnerWorkflow>(
        Func<TSource, IEnumerable<TInnerSource>> getElements)
        where TInnerWorkflow : IWorkflow<TInnerSource>
        where TInnerSource : class
    {
        return this.Then<StartWorkflowsState<TSource, TInnerSource>>()
            .Input(s => s.ElementWorkflow, async (TSource _, TInnerWorkflow innerWorkflow, CancellationToken _) => innerWorkflow)
            .Input(s => s.Elements, v => getElements(v).ToList());
    }

    public IStateBuilder<TSource, TStatus, IfState> If<TService>(
        Func<TSource, TService, CancellationToken, ValueTask<bool>> condition,
        Action<IWorkflowBuilder<TSource, TStatus>> trueSetupWorkflowBuilder,
        Action<IWorkflowBuilder<TSource, TStatus>>? falseSetupWorkflowBuilder = null)
        where TService : notnull
    {
        var ifState = this
            .ThenInternal<IfState>(false)
            .Input(s => s.Condition, condition);

        foreach (var pair in new[]
                 {
                     (IfState.TrueEvent, trueSetupWorkflowBuilder),
                     (IfState.FalseEvent, falseSetupWorkflowBuilder ?? (_ => { }))
                 })
        {
            var innerDefinition = workflowDefinitionBuilder.CloneHeader();

            pair.Item2(new WorkflowBuilder<TSource, TStatus>(innerDefinition));

            workflowDefinitionBuilder.Attach(ifState.StateDefinitionBuilder, pair.Item1, innerDefinition);
        }

        return ifState;
    }

    public IStateBuilder<TSource, TStatus, SwitchState<TProperty>> Switch<TProperty, TService>(
        Func<TSource, TService, CancellationToken, ValueTask<TProperty>> selector,
        Action<IWorkflowBuilder<TSource, TStatus>> defaultCaseSetupWorkflowBuilder,
        params (TProperty CaseValue, Action<IWorkflowBuilder<TSource, TStatus>> CaseSetupWorkflowBuilder)[] cases)

        where TService : notnull
        where TProperty : notnull
    {
        var baseCasesList = cases
            .Select((pair, i) => new
            {
                Value = pair.CaseValue, Setup = pair.CaseSetupWorkflowBuilder,
                EventHeader = new EventHeader($"Case_{i}")
            })
            .ToList();

        var stateCaseDict = baseCasesList.ToDictionary(pair => pair.Value, pair => pair.EventHeader);

        var switchState = this
            .ThenInternal<SwitchState<TProperty>>(false)
            .Input(s => s.Value, selector)
            .Input(s => s.Cases, stateCaseDict);

        var allEventCases = baseCasesList.Select(pair => new { pair.Setup, pair.EventHeader })
            .Concat([new { Setup = defaultCaseSetupWorkflowBuilder, EventHeader = SwitchState<TService>.DefaultCaseEvent }])
            .ToList();

        foreach (var eventCase in allEventCases)
        {
            var innerDefinition = workflowDefinitionBuilder.CloneHeader();

            eventCase.Setup(new WorkflowBuilder<TSource, TStatus>(innerDefinition));

            workflowDefinitionBuilder.Attach(switchState.StateDefinitionBuilder, eventCase.EventHeader, innerDefinition);
        }

        return switchState;
    }

    public IStateBuilder<TSource, TStatus, ParallelForeachState<TSource, TElement>> ParallelForeach<TElement, TService>(
        Func<TSource, TService, IAsyncEnumerable<TElement>> getElements,
        Action<IWorkflowBuilder<SourceItem<TSource, TElement>, Ignore>> setupIteratorBuilder)
        where TService : notnull
    {
        var iteratorWorkflow = new ActionBuildWorkflow<SourceItem<TSource, TElement>, Ignore>(setupIteratorBuilder);

        return this.Then<ParallelForeachState<TSource, TElement>>()
            .WithSubWorkflow([LazyHelper.Create<IWorkflowDefinitionBuilder>(() => iteratorWorkflow.Definition)])
            .Input(s => s.Elements, (TSource source, TService service, CancellationToken ct) => getElements(source, service).ToListAsync(ct))
            .Input(s => s.ElementWorkflow, _ => iteratorWorkflow.Definition);
    }

    public IStateBuilder<TSource, TStatus, ParallelState<TSource>> Parallel(
        params Action<IWorkflowBuilder<TSource, TStatus>>[] setupForks)
    {
        var subWorkflows = setupForks.Select(setupFork => new ForkBuildWorkflow<TSource, TStatus>(setupFork, workflowDefinitionBuilder)).ToArray();

        var lazyDefinitions = LazyHelper.Create(() => subWorkflows.Select(sw => sw.Definition).ToArray());

        return this.Then<ParallelState<TSource>>()
            .WithSubWorkflow([.. subWorkflows.Select(v => LazyHelper.Create<IWorkflowDefinitionBuilder>(() => v.Definition))])
            .Input(s => s.Forks, _ => lazyDefinitions.Value);
    }

    public IStateBuilder<TSource, TStatus, ForeachState<TSource, TElement>> Foreach<TElement, TService>(
        Func<TSource, TService, IAsyncEnumerable<TElement>> getElements,
        Action<IWorkflowBuilder<SourceItem<TSource, TElement>, Ignore>> setupIteratorBuilder)
        where TService : notnull
    {
        var iteratorWorkflow = new ActionBuildWorkflow<SourceItem<TSource, TElement>, Ignore>(setupIteratorBuilder);

        return this.Then<ForeachState<TSource, TElement>>()
            .WithSubWorkflow([LazyHelper.Create<IWorkflowDefinitionBuilder>(() => iteratorWorkflow.Definition)])
            .Input(s => s.Elements, (TSource source, TService service, CancellationToken ct) => getElements(source, service).ToListAsync(ct))
            .Input(s => s.ElementWorkflow, _ => iteratorWorkflow.Definition);
    }

    public IStateBuilder<TSource, TStatus, FinalState> Finish(Func<TSource, object?> getResult)
    {
        return this.Then<FinalState>()
            .Input(s => s.Result, getResult);
    }

    public IStateBuilder<TSource, TStatus, TaskState> Task(Action<ITaskBuilder<TSource, TStatus>> setup)
    {
        var taskState = this.ThenInternal<TaskState>(false);

        IStateBuilder<TSource, TStatus, TaskState> genTaskState = taskState;

        var taskBuilder = new TaskBuilder<TSource, TStatus>(workflowDefinitionBuilder, taskState);

        setup(taskBuilder);

        genTaskState.Input(s => s.CommandHeaders, taskBuilder.Commands);

        taskState.StateDefinitionBuilder.AdditionalInfo.Add(TaskState.CommandsKey, taskBuilder.Commands);

        return taskState;
    }
}