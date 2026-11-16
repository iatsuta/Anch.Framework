namespace Anch.Workflow.Tests.Condition;

public class ConditionWorkflowService
{
    public bool IsEven(ConditionWorkflowObject conditionObject) => conditionObject.Value % 2 == 0;
}