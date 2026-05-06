using Anch.Workflow.Domain;

namespace Anch.Workflow.Builder;

public interface ITaskBuilder<TSource, TStatus>
    where TSource : notnull
    where TStatus : struct
{
    ITaskBuilder<TSource, TStatus> AddCommand(EventHeader eventHeader, Action<IWorkflowBuilder<TSource, TStatus>> branchSetup);
}