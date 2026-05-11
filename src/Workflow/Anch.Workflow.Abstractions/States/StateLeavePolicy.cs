namespace Anch.Workflow.States;

public record StateLeavePolicy(string Name)
{
    public static StateLeavePolicy Forget { get; } = new(nameof(Forget));

    public static StateLeavePolicy TerminateChild { get; } = new(nameof(TerminateChild));
}