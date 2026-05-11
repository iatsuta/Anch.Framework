using Anch.Workflow.Engine;
using Anch.Workflow.Execution;
using Anch.Workflow.States;

namespace Anch.Workflow.Tests.Output;

public class BindingState : IState
{
    public int Value1 { get; set; }

    public int Value2 { get; set; }

    public int Result { get; set; }


    public async ValueTask<ExecutionResult> Run(IExecutionContext executionContext)
    {
        this.Result = this.Value1 + this.Value2;

        return new Done();
    }
}