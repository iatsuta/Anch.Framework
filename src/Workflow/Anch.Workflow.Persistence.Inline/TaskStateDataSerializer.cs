using Anch.Workflow.Domain;
using Anch.Workflow.Domain.Runtime;
using Anch.Workflow.States;

namespace Anch.Workflow.Persistence.Inline;

/// <summary>
/// Task state carries no data besides its status: the awaited commands are taken from the state definition.
/// </summary>
/// <typeparam name="TSource">The domain object that stores the state inline.</typeparam>
public class TaskStateDataSerializer<TSource> : IStateDataSerializer<TSource, TaskState>
    where TSource : class
{
    /// <inheritdoc/>
    public void Restore(TSource source, StateInstance stateInstance)
    {
        if (stateInstance.OutputProcessed)
        {
            return;
        }

        var commands = (IReadOnlyList<EventHeader>)stateInstance.Definition.AdditionalInfo[TaskState.CommandsKey];

        foreach (var header in commands)
        {
            stateInstance.WaitEvents.Add(new WaitEventInfo(header, null, stateInstance));
        }
    }

    /// <inheritdoc/>
    public void Store(TSource source, StateInstance stateInstance)
    {
    }
}
