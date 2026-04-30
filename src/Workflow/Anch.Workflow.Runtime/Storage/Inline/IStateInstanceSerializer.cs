using Anch.Workflow.Domain.Runtime;

namespace Anch.Workflow.Storage.Inline;

public interface IStateInstanceSerializer<in TSource, TState>;

public interface IStateInstanceSerializer<in TSource>
{
    StateInstance Deserialize(TSource source);

    void Serialize(StateInstance workflowInstance);
}