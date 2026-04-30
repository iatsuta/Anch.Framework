using Anch.Workflow.Engine;
using Anch.Workflow.ExecutionResult;
using Anch.Workflow.States._Base;

namespace Anch.Workflow.States;

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