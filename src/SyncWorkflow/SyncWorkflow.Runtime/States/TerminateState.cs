using SyncWorkflow.Engine;
using SyncWorkflow.ExecutionResult;
using SyncWorkflow.States._Base;

namespace SyncWorkflow.States;

public class TerminateState : IState
{
    public async Task<IExecutionResult> Run(IExecutionContext executionContext)
    {
        return new MultiExecutionResult([
            new PushEventResult(EventHeader.WorkflowTerminated, null),
            new PushEventResult(EventHeader.WorkflowFinished, null)
        ]);
    }
}