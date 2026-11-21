using Anch.Workflow.Domain;
using Anch.Workflow.Engine;
using Anch.Workflow.Execution;

namespace Anch.Workflow.States;

public class FinalState : IState
{
    public object? Result { get; set; }

    public async ValueTask<ExecutionResult> Run(IExecutionContext executionContext)
    {
        return new PushEventResult(EventHeader.WorkflowFinished, null, this.Result);
    }
}