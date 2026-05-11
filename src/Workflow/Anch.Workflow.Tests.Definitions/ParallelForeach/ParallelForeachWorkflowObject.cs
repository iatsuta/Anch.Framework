namespace Anch.Workflow.Tests.ParallelForeach;

public class ParallelForeachWorkflowObject
{
    public List<ParallelForeachItemWorkflowObject> Items { get; set; } = [];

    public int Result { get; set; }

    public int ExtraAddToResult { get; set; }
}