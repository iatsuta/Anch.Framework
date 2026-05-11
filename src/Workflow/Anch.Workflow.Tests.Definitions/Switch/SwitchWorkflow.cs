using Anch.Core;
using Anch.Workflow.Building;
using Anch.Workflow.Building.Default;

namespace Anch.Workflow.Tests.Switch;

public class SwitchWorkflow : BuildWorkflow<SwitchWorkflowObject>
{
    protected override void Build(IWorkflowBuilder<SwitchWorkflowObject, Ignore> builder) =>

        builder
            .Switch(wfObj => wfObj.Value,

                defaultCaseBuilder => defaultCaseBuilder.Then(wfObj => wfObj.Result = "DefaultCase"),

                (1, caseBuilder => caseBuilder.Then(wfObj => wfObj.Result = "Case 1")),
                (2, caseBuilder => caseBuilder.Then(wfObj => wfObj.Result = "Case 2")),
                (3, caseBuilder => caseBuilder.Then(wfObj => wfObj.Result = "Case 3")))

            .Then(wfObj => wfObj.Result = $"{wfObj.Value} {wfObj.Result}");
}