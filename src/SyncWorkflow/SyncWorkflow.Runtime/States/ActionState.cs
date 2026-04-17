using SyncWorkflow.Engine;
using SyncWorkflow.ExecutionResult;
using SyncWorkflow.States._Base;

namespace SyncWorkflow.States;

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