using Anch.Workflow.States.Output;

namespace Anch.Workflow.Builder;

public static class WorkflowBuilderExtensions
{
    public static IStateBuilder<TSource, TStatus, WriteLineState> WriteLine<TSource, TStatus>(this IWorkflowBuilder<TSource, TStatus> builder, string message)
        where TSource : notnull
        where TStatus : struct =>

        builder.WriteLine(_ => message);


    public static IStateBuilder<TSource, TStatus, WriteLineState> WriteLine<TSource, TStatus>(this IWorkflowBuilder<TSource, TStatus> builder,
        Func<TSource, string> getMessage)
        where TSource : notnull
        where TStatus : struct =>

        builder
            .Then<WriteLineState>()
            .Input(s => s.Message, getMessage);
}