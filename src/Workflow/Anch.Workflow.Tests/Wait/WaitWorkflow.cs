using Anch.Workflow.Builder;
using Anch.Workflow.Builder.Default;
using Anch.Workflow.States;

namespace Anch.Workflow.Tests.Wait;

public class WaitWorkflow : BuildWorkflow<WaitWorkflowSource>
{
    public const string WaitEventName = "TestWaitEvent";

    public const int WaitEventData = 123;

    protected override void Build(IWorkflowBuilder<WaitWorkflowSource> builder)
    {
        builder
            .Then<WaitEventState>()
            .WithStatus(WaitWorkflowStatus.ExampleWaitEvent)
            .Input(s => s.Event, new EventHeader(WaitEventName))
            .Input(s => s.ExpectedData, WaitEventData);
    }
}