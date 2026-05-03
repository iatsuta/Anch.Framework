using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Linq.Expressions;

using Anch.Core;
using Anch.Workflow.Domain;
using Anch.Workflow.Domain.Definition;
using Anch.Workflow.States;

namespace Anch.Workflow.Builder.Default.DomainDefinition;

public class WorkflowDefinition : IWorkflowDefinition
{
    private readonly List<StateDefinition> statesWithAutoFinish = [];

    private bool isAutoFinalStart = true;

    public WorkflowDefinition(Type sourceType)
    {
        this.SourceType = sourceType;

        this.DefaultFinalState = this.CreateDefaultFinalState();
        this.StartState = this.DefaultFinalState;
        this.TerminateState = this.CreateTerminateState();

        this.States.Add(this.DefaultFinalState);
    }

    public WorkflowDefinitionIdentity Identity { get; set; } = null!;

    public LambdaExpression? StatusProperty { get; set; }

    public LambdaExpression? VersionProperty { get; set; }

    public bool IsAutoIdentity { get; set; } = true;

    public WorkflowDomainBindingInfo DomainBindingInfo => field ??= this.BuildDomainBindingInfo();

    public bool InTechnical { get; set; }

    public Type SourceType { get; }

    public List<StateDefinition> States { get; set; } = [];

    public StateDefinition StartState { get; set; }

    public StateDefinition DefaultFinalState { get; set; }

    public StateDefinition TerminateState { get; set; }

    public Dictionary<string, object> Settings { get; set; } = new();

    IStateDefinition IWorkflowDefinition.StartState => this.StartState;

    IStateDefinition IWorkflowDefinition.TerminateState => this.TerminateState;

    IStateDefinition IWorkflowDefinition.DefaultFinalState => this.DefaultFinalState;

    ImmutableList<IStateDefinition> IWorkflowDefinition.States => field ??= [.. this.States];

    FrozenDictionary<string, object> IWorkflowDefinition.Settings => field ??= this.Settings.ToFrozenDictionary();

    public WorkflowDefinition HeaderClone()
    {
        return new WorkflowDefinition(this.SourceType);
    }

    public void AddState(StateDefinition stateDefinition)
    {
        this.InsertPreLast(stateDefinition);

        this.UpdateAutoFinish(stateDefinition);

        if (this.isAutoFinalStart)
        {
            this.isAutoFinalStart = false;

            this.StartState = stateDefinition;
        }
    }

    public void Attach(StateDefinition stateDefinition, EventHeader attachEvent, WorkflowDefinition branch)
    {
        if (stateDefinition.Workflow != this)
        {
            throw new InvalidOperationException();
        }

        var eventDefinition = new EventDefinition { Header = attachEvent };

        stateDefinition.Events.Add(eventDefinition);

        branch.TryAddEmptyState();

        stateDefinition.Transitions.Add(new TransitionDefinition { Event = eventDefinition, To = branch.StartState });

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

    private void InsertPreLast(StateDefinition stateDefinition)
    {
        this.States.Insert(this.States.Count - 1, stateDefinition);
    }

    public void UpdateAutoFinish(StateDefinition newLastStateDefinition)
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
            newLastStateDefinition.Transitions.Add(new TransitionDefinition { Event = newLastStateDefinition.Events[0], To = this.DefaultFinalState });

            this.statesWithAutoFinish.Add(newLastStateDefinition);
        }
    }

    private void TryAddEmptyState()
    {
        if (this.isAutoFinalStart)
        {
            var emptyState = new StateDefinition
            {
                Name = StateDefinition.SystemEmptyName,
                StateType = typeof(EmptyState),
                Workflow = this
            };

            emptyState.Events.Add(new EventDefinition
            {
                Header = EventHeader.StateDone
            });

            this.AddState(emptyState);
        }
    }

    private StateDefinition CreateDefaultFinalState()
    {
        var finalState = new StateDefinition
        {
            Name = StateDefinition.SystemFinalName,
            StateType = typeof(FinalState),
            Workflow = this
        };

        finalState.Events.Add(new EventDefinition { Header = EventHeader.WorkflowFinished });

        return finalState;
    }

    private StateDefinition CreateTerminateState()
    {
        var terminateState = new StateDefinition
        {
            Name = StateDefinition.SystemTerminateName,
            StateType = typeof(TerminateState),
            Workflow = this
        };

        terminateState.Events.Add(new EventDefinition { Header = EventHeader.WorkflowTerminated });

        terminateState.Events.Add(new EventDefinition { Header = EventHeader.WorkflowFinished });

        return terminateState;
    }

    public void Optimize()
    {
        foreach (var state in this.States.ToList())
        {
            if (state.Name == StateDefinition.SystemEmptyName && state.StateType == typeof(EmptyState))
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
    }

    public void ReplaceAutoNames((StateDefinition State, int? Index)? ownerInfo = null)
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
                subWfPair.SubWf.BaseDefinition.ReplaceAutoNames((statePair.State, statePair.State.SubWorkflows.Count == 1 ? null : subWfPair.Index));
            }
        }
    }

    private WorkflowDomainBindingInfo BuildDomainBindingInfo()
    {
        if (this.StatusProperty == null)
        {
            return new Func<WorkflowDomainBindingInfo<object>>(this.BuildDomainBindingInfoGeneric<object>)
                .CreateGenericMethod(this.SourceType)
                .Invoke<WorkflowDomainBindingInfo>(this);
        }
        else
        {
            return new Func<WorkflowDomainBindingInfo<object, Ignore>>(this.BuildDomainBindingInfoGeneric<object, Ignore>)
                .CreateGenericMethod(this.SourceType, this.StatusProperty.ReturnType)
                .Invoke<WorkflowDomainBindingInfo>(this);
        }
    }

    private WorkflowDomainBindingInfo<TSource> BuildDomainBindingInfoGeneric<TSource>() =>

        new() { Version = this.TryGetVersionPropertyAccessors<TSource>() };

    private WorkflowDomainBindingInfo<TSource, TStatus> BuildDomainBindingInfoGeneric<TSource, TStatus>() =>

        new()
        {
            Version = this.TryGetVersionPropertyAccessors<TSource>(),
            Status = new PropertyAccessors<TSource, TStatus>((Expression<Func<TSource, TStatus>>)this.StatusProperty!)
        };

    private PropertyAccessors<TSource, long>? TryGetVersionPropertyAccessors<TSource>() =>

        this.VersionProperty == null
            ? null
            : new PropertyAccessors<TSource, long>(
                this.VersionProperty as Expression<Func<TSource, long>> ?? throw new InvalidOperationException("Invalid version property"));

    public override string ToString() => this.Identity.ToString();
}