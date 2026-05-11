using Anch.Core;
using Anch.Workflow.Building;
using Anch.Workflow.Building.Default;

namespace Anch.Workflow.Tests.Output;

public class BindingWorkflow : BuildWorkflow<BindingWorkflowObject>
{
    protected override void Build(IWorkflowBuilder<BindingWorkflowObject, Ignore> builder) =>

        builder.Then<BindingState>()
            .Input(s => s.Value1, wfObj => wfObj.Value1)
            .Input(s => s.Value2, wfObj => wfObj.Value2)
            .Output(wfObj => wfObj.Result, s => s.Result);
}