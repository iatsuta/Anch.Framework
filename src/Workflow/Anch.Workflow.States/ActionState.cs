using Anch.Workflow.Engine;
using Anch.Workflow.Execution;

namespace Anch.Workflow.States;

public class ActionState<TSource, TService>(TService service) : IState
    where TService : notnull
{
    public Func<TSource, TService, CancellationToken, ValueTask> Action { get; set; } = null!;

    public async ValueTask<ExecutionResult> Run(IExecutionContext executionContext)
    {
        await this.Action((TSource)executionContext.Source, service, executionContext.CancellationToken);

        return new Done();
    }
}