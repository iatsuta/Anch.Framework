namespace SyncWorkflow.Builder.Default;

public class IteratorWorkflow<TSource, TElement>(
    Action<IWorkflowBuilder<(TSource Source, TElement Element)>> setupIteratorBuilder)
    : BuildWorkflow<(TSource, TElement)>
{
    protected override void Build(IWorkflowBuilder<(TSource, TElement)> builder)
    {
        setupIteratorBuilder(builder);
    }
}