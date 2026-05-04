using System.Collections.Immutable;

using Anch.Workflow.Domain.Runtime;

namespace Anch.Workflow.Execution;

public record WorkflowProcessResult(
    ImmutableList<WorkflowInstance> Modified,
    ImmutableStack<UnprocessedStateResultBase> Unprocessed)
{
    public WorkflowProcessResult(UnprocessedStateResultBase unprocessedStateResult)
        : this([], [unprocessedStateResult])
    {
    }

    public static WorkflowProcessResult operator +(WorkflowProcessResult result1, WorkflowProcessResult result2)
    {
        if (ReferenceEquals(result1, Empty))
        {
            return result2;
        }
        else if (ReferenceEquals(result2, Empty))
        {
            return result1;
        }
        else
        {
            var newModified = result1.Modified;

            foreach (var otherModified in result2.Modified)
            {
                if (!newModified.Contains(otherModified))
                {
                    newModified = newModified.Add(otherModified);
                }
            }

            var newUnprocessed = result2.Unprocessed;

            foreach (var item in result1.Unprocessed)
            {
                newUnprocessed = newUnprocessed.Push(item);
            }

            return new WorkflowProcessResult(newModified, newUnprocessed);
        }
    }

    public static WorkflowProcessResult Empty { get; } = new([], []);
}