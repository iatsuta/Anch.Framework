using SyncWorkflow.States.Output;

namespace SyncWorkflow.Builder;

public static class WorkflowBuilderExtensions
{
    public static IStateBuilder<TSource, WriteLineState> WriteLine<TSource>(this IWorkflowBuilder<TSource> builder, string message)
    {
        return builder.WriteLine(_ => message);
    }

    public static IStateBuilder<TSource, WriteLineState> WriteLine<TSource>(this IWorkflowBuilder<TSource> builder, Func<TSource, string> getMessage)
    {
        return builder
            .Then<WriteLineState>()
            .Input(s => s.Message, getMessage);
    }
}