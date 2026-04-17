using SyncWorkflow.Engine;
using SyncWorkflow.ExecutionResult;
using SyncWorkflow.States._Base;

namespace SyncWorkflow.States;

public class EmptyState : IState
{
    public async Task<IExecutionResult> Run(IExecutionContext executionContext)
    {
        return new Done();
    }
}