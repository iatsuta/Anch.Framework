using SyncWorkflow.Domain.Definition;
using SyncWorkflow.Engine;

namespace SyncWorkflow.Domain.Runtime;

public class StateInstance
{
    public Guid Id { get; set; }

    public StateInstanceFullIdentity Identity => new(this.Id, this.Workflow.Definition.Identity);

    public bool IsActual => this.IsCurrent
                            && this.Workflow.Status.Role != WorkflowStatusRole.Finished
                            && !this.OutputProcessed;

    public bool IsCurrent => this.Workflow.CurrentState == this;

    public bool InputProcessed { get; set; }

    public bool OutputProcessed { get; set; }

    public WorkflowInstance Workflow { get; set; } = null!;

    public IStateDefinition Definition { get; set; } = null!;

    public List<WorkflowInstance> Child { get; set; } = [];

    public HashSet<WaitEventInfo> WaitEvents { get; set; } = [];


    public void RegisterWaitEvent(WaitEventInfo eventInfo)
    {
        this.ValidateCurrentState(eventInfo.TargetState);

        if (!this.WaitEvents.Add(eventInfo))
        {
            throw new InvalidOperationException("Duplicate event");
        }
    }

    public List<WaitEventInfo> ReleaseWaitEvents()
    {
        var result = this.WaitEvents.ToList();

        this.WaitEvents.Clear();

        return result;
    }

    private void ValidateCurrentState(StateInstance stateInstance)
    {
        if (!this.IsCurrent || this != stateInstance)
        {
            throw new InvalidOperationException("Not current state");
        }
    }
}