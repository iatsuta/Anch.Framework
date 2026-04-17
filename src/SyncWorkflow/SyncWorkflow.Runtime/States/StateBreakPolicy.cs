using SyncWorkflow.Domain.Runtime;
using SyncWorkflow.Engine;

namespace SyncWorkflow.States;

public class StateBreakPolicy(Func<IExecutionContext, Task<bool>> func)
{
    public async Task<bool> CanBreak(IExecutionContext executionContext)
    {
        return await func(executionContext);
    }


    public static StateBreakPolicy WaitAll { get; } = CreateCondition(child => child.All(wi => wi.Status.Role == WorkflowStatusRole.Finished));


    public static StateBreakPolicy WaitAny { get; } = CreateCondition(child => child.Any(wi => wi.Status.Role == WorkflowStatusRole.Finished));

    public static StateBreakPolicy CreateCondition(Func<IEnumerable<WorkflowInstance>, bool> condition)
    {
        return new StateBreakPolicy(async executionContext => condition(executionContext.StateInstance.Child));
    }

    public static StateBreakPolicy AnyFinishedItem<TElement>(Func<TElement, bool> condition)
    {
        return CreateCondition(child => child.Any(subWf => subWf.Status.Role == WorkflowStatusRole.Finished && condition((TElement)subWf.Source)));
    }
}