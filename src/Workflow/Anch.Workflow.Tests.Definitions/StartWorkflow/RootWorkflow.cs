using Anch.Core;
using Anch.Workflow.Building;
using Anch.Workflow.Building.Default;
using Anch.Workflow.Tests.Wait;

namespace Anch.Workflow.Tests.StartWorkflow;

public class RootWorkflow : BuildWorkflow<WaitWorkflowSource>
{
    protected override void Build(IWorkflowBuilder<WaitWorkflowSource, Ignore> builder) =>

        builder.StartWorkflow<WaitWorkflowSource, WaitWorkflow>(v => v);
}