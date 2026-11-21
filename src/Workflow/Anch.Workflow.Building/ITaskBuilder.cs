using Anch.Workflow.Domain;

namespace Anch.Workflow.Building;

public interface ITaskBuilder<TSource, TStatus>
    where TSource : class
    where TStatus : struct
{
    ITaskBuilder<TSource, TStatus> AddCommand(EventHeader eventHeader, Action<IWorkflowBuilder<TSource, TStatus>> branchSetup);
}