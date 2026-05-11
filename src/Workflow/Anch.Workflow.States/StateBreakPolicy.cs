using Anch.Workflow.Domain.Runtime;
using Anch.Workflow.Engine;

namespace Anch.Workflow.States;

public class StateBreakPolicy(Func<IExecutionContext, ValueTask<bool>> func)
{
    public async ValueTask<bool> CanBreak(IExecutionContext executionContext)
    {
        return await func(executionContext);
    }


    public static StateBreakPolicy WaitAll { get; } = CreateCondition(child => child.All(wi => wi.Status.Role == WorkflowStatusRole.Finished));


    public static StateBreakPolicy WaitAny { get; } = CreateCondition(child => child.Any(wi => wi.Status.Role == WorkflowStatusRole.Finished));

    public static StateBreakPolicy CreateCondition(Func<IEnumerable<WorkflowInstance>, bool> condition)
    {
        return new StateBreakPolicy(async executionContext => condition(executionContext.StateInstance.Children));
    }

    public static StateBreakPolicy AnyFinishedItem<TElement>(Func<TElement, bool> condition)
    {
        return CreateCondition(child => child.Any(subWf => subWf.Status.Role == WorkflowStatusRole.Finished && condition((TElement)subWf.Source)));
    }
}