using Anch.Workflow.Domain.Runtime;
using Anch.Workflow.States;

namespace Anch.Workflow.Storage.Inline;

public class TaskStateInstanceSerializer : IStateInstanceSerializer<TaskState>
{
    public StateInstance Deserialize(TaskState source)
    {
        throw new NotImplementedException();
    }

    public void Serialize(StateInstance workflowInstance)
    {
        throw new NotImplementedException();
    }
}