using Anch.Workflow.Engine;
using Anch.Workflow.Execution;

namespace Anch.Workflow.States;

public class EmptyState : IState
{
    public async ValueTask<ExecutionResult> Run(IExecutionContext executionContext)
    {
        return new Done();
    }
}