using Anch.Workflow.Engine;
using Anch.Workflow.ExecutionResult;
using Anch.Workflow.States._Base;

namespace Anch.Workflow.States;

public class ActionState<TSource, TService>(TService service) : IState
    where TService : notnull
{
    public Func<TSource, TService, CancellationToken, Task> Action { get; set; } = null!;

    public async Task<IExecutionResult> Run(IExecutionContext executionContext)
    {
        await this.Action((TSource)executionContext.Source, service, executionContext.CancellationToken);

        return new Done();
    }
}