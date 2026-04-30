using Anch.Workflow.Builder;
using Anch.Workflow.Builder.Default;
using Anch.Workflow.States;

namespace Anch.Workflow.Tests.ParallelForeach;

public class ParallelForeachItemWorkflow : BuildWorkflow<ParallelForeachItemWorkflowObject>
{
    public static readonly EventHeader TestItemWaitEvent = new(nameof(TestItemWaitEvent));

    protected override void Build(IWorkflowBuilder<ParallelForeachItemWorkflowObject> builder)
    {
        builder.If(wfObj => wfObj.Value % 2 == 0,

                trueBuilder => trueBuilder
                                .Then<WaitEventState>()
                                    .Input(s => s.Event, TestItemWaitEvent)
                                    .Output(wfObj => wfObj.EventValue, s => (int)s.ReceivedData!)
                                .Then(_ => { }));
    }
}