namespace SyncWorkflow.Builder.Default;

public class ForkWorkflow<TSource>(Action<IWorkflowBuilder<TSource>> setupBuilder) : BuildWorkflow<TSource>
{
    protected override void Build(IWorkflowBuilder<TSource> builder)
    {
        setupBuilder(builder);
    }
}