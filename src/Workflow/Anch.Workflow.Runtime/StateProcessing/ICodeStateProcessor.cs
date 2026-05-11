using Anch.Workflow.States;

namespace Anch.Workflow.StateProcessing;

public interface ICodeStateProcessor
{
    IState CodeState { get; }

    void SetStatus();

    ValueTask BindInput(CancellationToken ct);

    ValueTask BindOutput(CancellationToken ct);
}