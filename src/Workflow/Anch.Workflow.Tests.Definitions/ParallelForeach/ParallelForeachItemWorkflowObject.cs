namespace Anch.Workflow.Tests.ParallelForeach;

public class ParallelForeachItemWorkflowObject
{
    public int Value { get; set; }

    public int EventValue { get; set; }

    public string Name { get; set; } = null!;

    public override string ToString() => this.Name;
}