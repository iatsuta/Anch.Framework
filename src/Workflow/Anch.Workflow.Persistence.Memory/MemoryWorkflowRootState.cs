using Anch.Workflow.Domain.Runtime;

namespace Anch.Workflow.Persistence.Memory;

public record MemoryWorkflowRootState
{
    public readonly Dictionary<WorkflowInstanceIdentity, WorkflowInstance> WorkflowInstances = [];

    public readonly Dictionary<StateInstanceIdentity, StateInstance> StateInstances = [];
}