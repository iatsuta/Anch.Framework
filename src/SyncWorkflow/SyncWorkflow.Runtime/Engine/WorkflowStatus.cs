namespace SyncWorkflow.Engine;

public record WorkflowStatus(string Name, WorkflowStatusRole Role)
{
    public WorkflowStatus(string name)
        : this(name, WorkflowStatusRole.Other)
    {
    }

    public static WorkflowStatus NotStarted { get; } = new (nameof(NotStarted));

    public static WorkflowStatus Runnable { get; } = new (nameof(Runnable));

    public static WorkflowStatus Finished { get; } = new (nameof(Finished), WorkflowStatusRole.Finished);

    public static WorkflowStatus WaitEvent { get; } = new (nameof(WaitEvent));

    public static WorkflowStatus Suspended { get; } = new (nameof(Suspended));

    public static WorkflowStatus Terminated { get; } = new (nameof(Terminated), WorkflowStatusRole.Finished);
}

public record WorkflowStatusRole(string Name)
{
    public static WorkflowStatusRole Finished { get; } = new (nameof(Finished));

    public static WorkflowStatusRole Other { get; } = new (nameof(Other));
}