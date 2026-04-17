using SyncWorkflow.Engine;
using SyncWorkflow.ExecutionResult;
using SyncWorkflow.States._Base;

namespace SyncWorkflow.States.Output;

public class WriteLineState(IDefaultOutput output) : IState
{
    public string Message { get; set; } = null!;

    public async Task<IExecutionResult> Run(IExecutionContext executionContext)
    {
        await output.TextWriter.WriteLineAsync(this.Message);

        return new Done();
    }
}