using Anch.Workflow.Engine;
using Anch.Workflow.ExecutionResult;
using Anch.Workflow.States._Base;

namespace Anch.Workflow.States.Output;

public class WriteLineState(IDefaultOutput output) : IState
{
    public string Message { get; set; } = null!;

    public async Task<IExecutionResult> Run(IExecutionContext executionContext)
    {
        await output.TextWriter.WriteLineAsync(this.Message);

        return new Done();
    }
}