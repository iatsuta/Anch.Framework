using System.Collections.Immutable;

namespace Anch.Workflow.Execution;

public record MultiExecutionResult(ImmutableArray<ExecutionResult> ExecutionResults) : ExecutionResult
{
    public override bool LeaveState => this.ExecutionResults.All(er => er.LeaveState);
}