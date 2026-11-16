namespace Anch.Workflow.Domain.Runtime;

public record WorkflowStatusRole(string Name)
{
    public static WorkflowStatusRole Finished { get; } = new(nameof(Finished));

    public static WorkflowStatusRole Other { get; } = new(nameof(Other));
}