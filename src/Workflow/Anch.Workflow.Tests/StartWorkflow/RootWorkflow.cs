using Anch.Workflow.Builder;
using Anch.Workflow.Builder.Default;
using Anch.Workflow.Tests.Wait;

namespace Anch.Workflow.Tests.StartWorkflow;

public class RootWorkflow : BuildWorkflow<WaitWorkflowSource>
{
    protected override void Build(IWorkflowBuilder<WaitWorkflowSource> builder)
    {
        builder.StartWorkflow<WaitWorkflowSource, WaitWorkflow>(v => v);
    }
}