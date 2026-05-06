using Anch.Core;
using Anch.Workflow.Domain;
using Anch.Workflow.Domain.Definition;
using Anch.Workflow.States;

namespace Anch.Workflow.Builder.Default.DomainDefinition;

public class WorkflowDefinitionBuilder<TSource, TStatus> : IWorkflowDefinition<TSource, TStatus>, IWorkflowDefinitionBuilder
    where TSource : notnull
    where TStatus : struct
{
    private readonly List<IStateDefinitionBuilder<TSource, TStatus>> statesWithAutoFinish = [];

    private bool isAutoFinalStart = true;

    public WorkflowDefinitionBuilder()
    {
        this.DefaultFinalState = this.CreateDefaultFinalState();
        this.StartState = this.DefaultFinalState;
        this.TerminateState = this.CreateTerminateState();

        this.States.Add(this.DefaultFinalState);
    }

    public PropertyAccessors<TSource, TStatus>? StatusAccessors { get; set; }

    public PropertyAccessors<TSource, long>? VersionAccessors { get; set; }

    public WorkflowDefinitionIdentity Identity { get; set; } = null!;

    public bool IsAutoIdentity { get; set; } = true;

    public bool IsRoot { get; set; }

    public bool AllowReplaceAutoNames { get; set; }

    public List<IStateDefinitionBuilder<TSource, TStatus>> States { get; set; } = [];

    public IStateDefinitionBuilder<TSource, TStatus> StartState { get; set; }

    public IStateDefinitionBuilder<TSource, TStatus> DefaultFinalState { get; set; }

    public IStateDefinitionBuilder<TSource, TStatus> TerminateState { get; set; }

    public Dictionary<string, object> Settings { get; set; } = [];

    public WorkflowDefinitionBuilder<TSource, TStatus> CloneHeader()
    {
        return new WorkflowDefinitionBuilder<TSource, TStatus>();
    }

    public void AddState(IStateDefinitionBuilder<TSource, TStatus> stateDefinition)
    {
        this.InsertPreLast(stateDefinition);

        this.UpdateAutoFinish(stateDefinition);

        if (this.isAutoFinalStart)
        {
            this.isAutoFinalStart = false;

            this.StartState = stateDefinition;
        }
    }

    public void Attach(IStateDefinitionBuilder<TSource, TStatus> stateDefinition, EventHeader attachEvent, WorkflowDefinitionBuilder<TSource, TStatus> branch)
    {
        if (stateDefinition.Workflow != this)
        {
            throw new InvalidOperationException();
        }

        var eventDefinition = new EventDefinitionBuilder { Header = attachEvent };

        stateDefinition.Events.Add(eventDefinition);

        branch.TryAddEmptyState();

        stateDefinition.Transitions.Add(new TransitionDefinitionBuilder<TSource, TStatus> { Event = eventDefinition, To = branch.StartState });

        this.statesWithAutoFinish.AddRange(branch.statesWithAutoFinish);

        foreach (var branchState in branch.States)
        {
            if (branchState != branch.DefaultFinalState)
            {
                branchState.Workflow = this;

                foreach (var branchStateTransition in branchState.Transitions)
                {
                    if (branchStateTransition.To == branch.DefaultFinalState)
                    {
                        branchStateTransition.To = this.DefaultFinalState;
                    }
                }

                this.InsertPreLast(branchState);
            }
        }
    }

    private void InsertPreLast(IStateDefinitionBuilder<TSource, TStatus> stateDefinition)
    {
        this.States.Insert(this.States.Count - 1, stateDefinition);
    }

    public void UpdateAutoFinish(IStateDefinitionBuilder<TSource, TStatus> newLastStateDefinition)
    {
        foreach (var stateDefinition in this.statesWithAutoFinish)
        {
            var autoFinishTransition = stateDefinition.Transitions.Single();

            if (autoFinishTransition.To != this.DefaultFinalState)
            {
                throw new InvalidOperationException();
            }

            autoFinishTransition.To = newLastStateDefinition;
        }

        this.statesWithAutoFinish.Clear();

        if (!newLastStateDefinition.Transitions.Any() && newLastStateDefinition.Events.Count == 1 &&
            newLastStateDefinition.Events[0].Header == EventHeader.StateDone)
        {
            newLastStateDefinition.Transitions.Add(new TransitionDefinitionBuilder<TSource, TStatus> { Event = newLastStateDefinition.Events[0], To = this.DefaultFinalState });

            this.statesWithAutoFinish.Add(newLastStateDefinition);
        }
    }

    private void TryAddEmptyState()
    {
        if (this.isAutoFinalStart)
        {
            var emptyState = new StateDefinitionBuilder<TSource, TStatus, EmptyState>
            {
                Name = StateDefinitionBuilder.SystemEmptyName,
                Workflow = this
            };

            emptyState.Events.Add(new EventDefinitionBuilder
            {
                Header = EventHeader.StateDone
            });

            this.AddState(emptyState);
        }
    }

    private IStateDefinitionBuilder<TSource, TStatus> CreateDefaultFinalState()
    {
        var finalState = new StateDefinitionBuilder<TSource, TStatus, FinalState>
        {
            Name = StateDefinitionBuilder.SystemFinalName,
            Workflow = this
        };

        finalState.Events.Add(new EventDefinitionBuilder { Header = EventHeader.WorkflowFinished });

        return finalState;
    }

    private IStateDefinitionBuilder<TSource, TStatus> CreateTerminateState()
    {
        var terminateState = new StateDefinitionBuilder<TSource, TStatus, TerminateState>
        {
            Name = StateDefinitionBuilder.SystemTerminateName,
            Workflow = this
        };

        terminateState.Events.Add(new EventDefinitionBuilder { Header = EventHeader.WorkflowTerminated });

        terminateState.Events.Add(new EventDefinitionBuilder { Header = EventHeader.WorkflowFinished });

        return terminateState;
    }

    public void Optimize()
    {
        foreach (var state in this.States.ToList())
        {
            if (state.Name == StateDefinitionBuilder.SystemEmptyName && state.StateType == typeof(EmptyState))
            {
                var nextState = state.Transitions.Single().To;

                this.States.Remove(state);

                foreach (var transition in this.States.SelectMany(s => s.Transitions))
                {
                    if (transition.To == state)
                    {
                        transition.To = nextState;
                    }
                }
            }
        }
    }

    public void Validate()
    {
        var invalidOwnerRequest =
            from state in this.States
            where state.Workflow != this
            select state;

        if (invalidOwnerRequest.Any())
        {
            throw new InvalidOperationException("Invalid owners");
        }

        var statesWithoutTransitions =

            from state in this.States

            from @event in state.Events

            where !@event.Header.IsGlobal && state.Transitions.Count(tr => tr.Event == @event) != 1

            select new { state, @event };

        if (statesWithoutTransitions.Any())
        {
            throw new InvalidOperationException("Invalid events");
        }

        if (typeof(TStatus) != typeof(Ignore) && this.StatusAccessors == null)
        {
            throw new InvalidOperationException($"{nameof(this.StatusAccessors)} must be initialized");
        }
    }

    public void ReplaceAutoNames((IStateDefinitionBuilder State, int? Index)? ownerInfo = null)
    {
        var stateDigitCount = (int)Math.Log10(this.States.Count);

        if (this.IsAutoIdentity && ownerInfo != null)
        {
            var newName = $"{ownerInfo.Value.State.Workflow.Identity.Name}-{ownerInfo.Value.State.Name}"
                          + ownerInfo.Value.Index.MaybeNullable(index => $"-{index}")
                          + $"-{this.Identity.Name}";

            this.Identity = new WorkflowDefinitionIdentity(newName);
            this.IsAutoIdentity = false;
        }

        foreach (var statePair in this.States.Select((state, index) => new { State = state, Index = index }))
        {
            if (statePair.State.IsAutoName)
            {
                statePair.State.Name = $"State-{statePair.Index.ToString($"D{stateDigitCount}")}";
                statePair.State.IsAutoName = false;
            }

            foreach (var subWfPair in statePair.State.SubWorkflows.Select((subWf, index) => new { SubWf = subWf, Index = index }))
            {
                subWfPair.SubWf.ReplaceAutoNames((statePair.State, statePair.State.SubWorkflows.Count == 1 ? null : subWfPair.Index));
            }
        }
    }

    public override string ToString() => this.Identity.ToString();


    IReadOnlyList<IStateDefinition<TSource, TStatus>> IWorkflowDefinition<TSource, TStatus>.States => this.States;

    IStateDefinition<TSource, TStatus> IWorkflowDefinition<TSource, TStatus>.StartState => this.StartState;

    IStateDefinition<TSource, TStatus> IWorkflowDefinition<TSource, TStatus>.DefaultFinalState => this.DefaultFinalState;

    IStateDefinition<TSource, TStatus> IWorkflowDefinition<TSource, TStatus>.TerminateState => this.TerminateState;


    IReadOnlyList<IStateDefinition> IWorkflowDefinition.States => this.States;

    IStateDefinition IWorkflowDefinition.StartState => this.StartState;

    IStateDefinition IWorkflowDefinition.TerminateState => this.TerminateState;

    IStateDefinition IWorkflowDefinition.DefaultFinalState => this.DefaultFinalState;


    IReadOnlyDictionary<string, object> IWorkflowDefinition.Settings => this.Settings;
}