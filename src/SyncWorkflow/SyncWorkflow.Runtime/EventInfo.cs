using SyncWorkflow.Domain.Runtime;

namespace SyncWorkflow;

public record WaitEventInfo(
    EventHeader Event,
    WorkflowInstance? SourceWorkflow,
    StateInstance TargetState,
    object? Data = null)
{
    public bool Release()
    {
        return this.TargetState.WaitEvents.Remove(this);
    }
}

public record PushEventInfo(
    EventHeader Event,
    WorkflowInstance? SourceWorkflow,
    StateInstance? TargetState,
    object? Data = null)
{
    public bool IsMatched(WaitEventInfo waitEventInfo)
    {
        return (waitEventInfo.Event == this.Event)
            && (waitEventInfo.TargetState == this.TargetState || this.TargetState == null)
            && (waitEventInfo.SourceWorkflow == null || this.SourceWorkflow == waitEventInfo.SourceWorkflow)
            && (waitEventInfo.Data == null || Equals(waitEventInfo.Data, this.Data));
    }
}