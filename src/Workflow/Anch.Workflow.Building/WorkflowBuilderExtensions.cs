using Anch.Workflow.States.Output;

namespace Anch.Workflow.Building;

public static class WorkflowBuilderExtensions
{
    public static IStateBuilder<TSource, TStatus, WriteLineState> WriteLine<TSource, TStatus>(this IWorkflowBuilder<TSource, TStatus> builder, string message)
        where TSource : class
        where TStatus : struct =>

        builder.WriteLine(_ => message);


    public static IStateBuilder<TSource, TStatus, WriteLineState> WriteLine<TSource, TStatus>(this IWorkflowBuilder<TSource, TStatus> builder,
        Func<TSource, string> getMessage)
        where TSource : class
        where TStatus : struct =>

        builder
            .Then<WriteLineState>()
            .Input(s => s.Message, getMessage);
}