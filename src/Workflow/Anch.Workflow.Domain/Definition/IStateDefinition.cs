namespace Anch.Workflow.Domain.Definition;

public interface IStateDefinition<TSource, TStatus, in TState> : IStateDefinition<TSource, TStatus>
    where TSource : notnull
    where TStatus : struct
{
    IReadOnlyList<Func<IServiceProvider, TSource, TState, CancellationToken, ValueTask>> InputActions { get; }

    IReadOnlyList<Func<IServiceProvider, TState, TSource, CancellationToken, ValueTask>> OutputActions { get; }

    Type IStateDefinition.StateType => typeof(TState);
}

public interface IStateDefinition<TSource, TStatus> : IStateDefinition
    where TSource : notnull
    where TStatus : struct
{
    new IWorkflowDefinition<TSource, TStatus> Workflow { get; }

    new IReadOnlyList<ITransitionDefinition<TSource, TStatus>> Transitions { get; }

    new TStatus? Status { get; }



    IWorkflowDefinition IStateDefinition.Workflow => this.Workflow;

    IReadOnlyList<ITransitionDefinition> IStateDefinition.Transitions => this.Transitions;

    object? IStateDefinition.Status => this.Status;
}

//public interface IStateDefinition<TSource> : IStateDefinition
//    where TSource : notnull
//{
//    new IWorkflowDefinition<TSource> Workflow { get; }

//    new IReadOnlyList<ITransitionDefinition<TSource>> Transitions { get; }


//    IWorkflowDefinition IStateDefinition.Workflow => this.Workflow;

//    IReadOnlyList<ITransitionDefinition> IStateDefinition.Transitions => this.Transitions;
//}

public interface IStateDefinition
{
    string Name { get; }

    object? Status { get; }

    Type StateType { get; }

    IWorkflowDefinition Workflow { get; }

    IReadOnlyList<ITransitionDefinition> Transitions { get; }

    IReadOnlyList<IWorkflowDefinition> SubWorkflows { get; }

    IReadOnlyDictionary<string, object> AdditionalInfo { get; }
}