using SyncWorkflow.Engine;
using SyncWorkflow.ExecutionResult;
using SyncWorkflow.States._Base;

namespace SyncWorkflow.States;

public class FinalState : IState
{
    public object? Result { get; set; }

    public async Task<IExecutionResult> Run(IExecutionContext executionContext)
    {
        return new PushEventResult(EventHeader.WorkflowFinished, null, this.Result);
    }
}