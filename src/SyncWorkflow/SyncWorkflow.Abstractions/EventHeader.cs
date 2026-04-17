namespace SyncWorkflow;

public record EventHeader(string Name, bool IsGlobal = false)
{
    public static EventHeader StateDone { get; } = new(nameof(StateDone));

    public static EventHeader WorkflowFinished { get; } = new(nameof(WorkflowFinished), true);

    public static EventHeader WorkflowTerminated { get; } = new(nameof(WorkflowTerminated), true);
}