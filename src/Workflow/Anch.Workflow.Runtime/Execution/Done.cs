using Anch.Workflow.Domain.Runtime;

namespace Anch.Workflow.Execution;

public record Done() : IExecutionResult
{
    public bool LeaveState { get; } = true;
}