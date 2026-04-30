using Anch.Workflow.Builder;
using Anch.Workflow.Builder.Default;

namespace Anch.Workflow.Tests.Output;

public class BindingWorkflow : BuildWorkflow<BindingWorkflowObject>
{
    protected override void Build(IWorkflowBuilder<BindingWorkflowObject> builder)
    {
        builder.Then<BindingState>()
            .Input(s => s.Value1, wfObj => wfObj.Value1)
            .Input(s => s.Value2, wfObj => wfObj.Value2)
            .Output(wfObj => wfObj.Result, s => s.Result);
    }
}