namespace SyncWorkflow.Builder;

public interface ITaskBuilder<TSource>
    where TSource : notnull
{
    ITaskBuilder<TSource> AddCommand(EventHeader eventHeader, Action<IWorkflowBuilder<TSource>> branchSetup);
}