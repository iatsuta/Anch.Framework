using System.Linq.Expressions;

using Anch.Workflow.Builder.Default.DomainDefinition;
using Anch.Workflow.Definition;
using Anch.Workflow.States;
using Anch.Workflow.States._Base;

namespace Anch.Workflow.Builder.Default;

public class WorkflowBuilder<TSource>(WorkflowDefinition workflow) : IWorkflowBuilder<TSource>
    where TSource : notnull
{
    protected readonly WorkflowDefinition Workflow = workflow;

    public IWorkflowBuilder<TSource> WithIdProperty(Expression<Func<TSource, Guid>> idPath)
    {
        throw new NotImplementedException();
        //this.Workflow.IdProperty = idPath;

        return this;
    }

    public IWorkflowBuilder<TSource> WithStatusProperty<TStatus>(Expression<Func<TSource, TStatus>> statusPath)
    {
        this.Workflow.StatusProperty = statusPath;

        return this;
    }

    public IWorkflowBuilder<TSource> WithVersionProperty<TStatus>(Expression<Func<TSource, Guid>> versionPath)
    {
        this.Workflow.VersionProperty = versionPath;

        return this;
    }

    public IWorkflowBuilder<TSource> WithSetting(string name, object value)
    {
        this.Workflow.Settings[name] = value;

        return this;
    }

    public IWorkflowBuilder<TSource> WithIdentity(WorkflowDefinitionIdentity identity)
    {
        this.Workflow.Identity = identity;
        this.Workflow.IsAutoIdentity = false;

        return this;
    }

    public IStateBuilder<TSource, TState> Then<TState>()
        where TState : IState
    {
        return this.ThenInternal<TState>(true);
    }

    private StateBuilder<TSource, TState> ThenInternal<TState>(bool addDoneEvent)
        where TState : IState
    {
        return new StateBuilder<TSource, TState>(this.Workflow, addDoneEvent);
    }

    public IWorkflowBuilder<TSource> Then(IStateBuilder state)
    {
        this.Workflow.UpdateAutoFinish(state.StateDefinition);

        return this;
    }

    public IStateBuilder<TSource, StartWorkflowState<TInnerSource>> StartWorkflow<TInnerSource, TWorkflow>(Func<TSource, TInnerSource> getInnerSource)
        where TWorkflow : IWorkflow<TInnerSource>
        where TInnerSource : notnull
    {
        return this.Then<StartWorkflowState<TInnerSource>>()
            .Input(s => s.InnerWorkflow, async (TSource _, TWorkflow workflow, CancellationToken _) => workflow)
            .Input(s => s.InnerSource, getInnerSource);
    }

    public IStateBuilder<TSource, StartWorkflowsState<TSource, TInnerSource>> StartWorkflows<TInnerSource, TWorkflow>(
        Func<TSource, IEnumerable<TInnerSource>> getElements)
        where TWorkflow : IWorkflow<TInnerSource>
    {
        return this.Then<StartWorkflowsState<TSource, TInnerSource>>()
            .Input(s => s.ElementWorkflow, async (TSource _, TWorkflow workflow, CancellationToken _) => workflow)
            .Input(s => s.Elements, getElements);
    }

    public IStateBuilder<TSource, IfState> If<TService>(
        Func<TSource, TService, CancellationToken, Task<bool>> condition,
        Action<IWorkflowBuilder<TSource>> trueSetupWorkflowBuilder,
        Action<IWorkflowBuilder<TSource>>? falseSetupWorkflowBuilder = null)
        where TService : notnull
    {
        var ifState = this
            .ThenInternal<IfState>(false)
            .Input(s => s.Condition, condition);

        foreach (var pair in new[]
                 {
                     (IfState.TrueEvent, trueSetupWorkflowBuilder),
                     (IfState.FalseEvent, falseSetupWorkflowBuilder ?? (_ => {}))
                 })
        {
            var innerDefinition = this.Workflow.HeaderClone();

            pair.Item2(new WorkflowBuilder<TSource>(innerDefinition));

            this.Workflow.Attach(ifState.StateDefinition, pair.Item1, innerDefinition);
        }

        return ifState;
    }

    public IStateBuilder<TSource, SwitchState<TProperty>> Switch<TProperty, TService>(
        Func<TSource, TService, CancellationToken, Task<TProperty>> selector,
        Action<IWorkflowBuilder<TSource>> defaultCaseSetupWorkflowBuilder,
        params (TProperty CaseValue, Action<IWorkflowBuilder<TSource>> CaseSetupWorkflowBuilder)[] cases)

        where TService : notnull
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
            var innerDefinition = this.Workflow.HeaderClone();

            eventCase.Setup(new WorkflowBuilder<TSource>(innerDefinition));

            this.Workflow.Attach(switchState.StateDefinition, eventCase.EventHeader, innerDefinition);
        }

        return switchState;
    }

    public IStateBuilder<TSource, ParallelForeachState<TSource, TElement>> ParallelForeach<TElement, TService>(
        Func<TSource, TService, CancellationToken, Task<IEnumerable<TElement>>> getElements,
        Action<IWorkflowBuilder<(TSource Source, TElement Element)>> setupIteratorBuilder)
        where TService : notnull
    {
        var iteratorWorkflow = new IteratorWorkflow<TSource, TElement>(setupIteratorBuilder);

        return this.Then<ParallelForeachState<TSource, TElement>>()
            .WithSubWorkflow([iteratorWorkflow])
            .Input(s => s.Elements, getElements)
            .Input(s => s.ElementWorkflow, iteratorWorkflow);
    }

    public IStateBuilder<TSource, ParallelState<TSource>> Parallel(
        params Action<IWorkflowBuilder<TSource>>[] setupForks)
    {
        var forks = setupForks
            .Select(setupFork => new ForkWorkflow<TSource>(setupFork))
            .ToArray();

        return this.Then<ParallelState<TSource>>()
            .WithSubWorkflow(forks)
            .Input(s => s.Forks, forks);
    }

    public IStateBuilder<TSource, ForeachState<TSource, TElement>> Foreach<TElement, TService>(
        Func<TSource, TService, CancellationToken, Task<IEnumerable<TElement>>> getElements,
        Action<IWorkflowBuilder<(TSource Source, TElement Element)>> setupIteratorBuilder)
        where TService : notnull
    {
        var iteratorWorkflow = new IteratorWorkflow<TSource, TElement>(setupIteratorBuilder);

        return this.Then<ForeachState<TSource, TElement>>()
            .WithSubWorkflow([iteratorWorkflow])
            .Input(s => s.Elements, getElements)
            .Input(s => s.ElementWorkflow, iteratorWorkflow);
    }

    public IStateBuilder<TSource, FinalState> Finish(Func<TSource, object?> getResult)
    {
        return this.Then<FinalState>()
            .Input(s => s.Result, getResult);
    }

    public IStateBuilder<TSource, TaskState> Task(Action<ITaskBuilder<TSource>> setup)
    {
        var taskState = this.ThenInternal<TaskState>(false);

        IStateBuilder<TSource, TaskState> genTaskState = taskState;

        var taskBuilder = new TaskBuilder<TSource>(this.Workflow, taskState);

        setup(taskBuilder);

        genTaskState.Input(s => s.CommandHeaders, taskBuilder.Commands);

        taskState.StateDefinition.AdditionalInfo.Add(TaskState.CommandsKey, taskBuilder.Commands);

        return taskState;
    }
}