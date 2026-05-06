using Anch.Workflow.Domain;
using Anch.Workflow.Engine;
using Anch.Workflow.Execution;

namespace Anch.Workflow.States;

public class TerminateState : IState
{
    public async ValueTask<ExecutionResult> Run(IExecutionContext executionContext)
    {
        return new MultiExecutionResult([
            new PushEventResult(EventHeader.WorkflowTerminated, null),
            new PushEventResult(EventHeader.WorkflowFinished, null)
        ]);
    }
}