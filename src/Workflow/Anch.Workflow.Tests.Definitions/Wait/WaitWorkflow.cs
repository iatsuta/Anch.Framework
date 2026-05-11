using Anch.Workflow.Building;
using Anch.Workflow.Building.Default;
using Anch.Workflow.Domain;
using Anch.Workflow.States;

namespace Anch.Workflow.Tests.Wait;

public class WaitWorkflow : BuildWorkflow<WaitWorkflowSource, WaitWorkflowStatus>
{
    public const string WaitEventName = "TestWaitEvent";

    public const int WaitEventData = 123;

    protected override void Build(IWorkflowBuilder<WaitWorkflowSource, WaitWorkflowStatus> builder) =>

        builder
            .WithStatusProperty(wfObj => wfObj.Status)
            .Then<WaitEventState>()
            .WithStatus(WaitWorkflowStatus.ExampleWaitEvent)
            .Input(s => s.Event, new EventHeader(WaitEventName))
            .Input(s => s.ExpectedData, WaitEventData);
}