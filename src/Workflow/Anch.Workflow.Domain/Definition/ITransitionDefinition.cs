namespace Anch.Workflow.Domain.Definition;

public interface ITransitionDefinition<TSource, TStatus> : ITransitionDefinition
    where TSource : notnull
    where TStatus : struct
{
    new IStateDefinition<TSource, TStatus> To { get; }

    //IStateDefinition<TSource> ITransitionDefinition<TSource>.To => this.To;

    IStateDefinition ITransitionDefinition.To => this.To;
}

//public interface ITransitionDefinition<TSource> : ITransitionDefinition
//    where TSource : notnull
//{
//    new IStateDefinition<TSource> To { get; }

//    IStateDefinition ITransitionDefinition.To => this.To;
//}

public interface ITransitionDefinition
{
    IEventDefinition Event { get; }

    IStateDefinition To { get; }
}