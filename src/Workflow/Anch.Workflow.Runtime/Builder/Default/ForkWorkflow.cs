namespace Anch.Workflow.Builder.Default;

public class ForkWorkflow<TSource>(Action<IWorkflowBuilder<TSource>> setupBuilder) : BuildWorkflow<TSource>
    where TSource : notnull
{
    protected override void Build(IWorkflowBuilder<TSource> builder)
    {
        setupBuilder(builder);
    }
}