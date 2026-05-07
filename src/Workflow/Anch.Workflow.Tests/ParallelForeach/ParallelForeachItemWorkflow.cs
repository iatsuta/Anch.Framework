using Anch.Core;
using Anch.Workflow.Building;
using Anch.Workflow.Building.Default;
using Anch.Workflow.Domain;
using Anch.Workflow.States;

namespace Anch.Workflow.Tests.ParallelForeach;

public class ParallelForeachItemWorkflow : BuildWorkflow<ParallelForeachItemWorkflowObject>
{
    public static readonly EventHeader TestItemWaitEvent = new(nameof(TestItemWaitEvent));

    protected override void Build(IWorkflowBuilder<ParallelForeachItemWorkflowObject, Ignore> builder)
    {
        builder.If(wfObj => wfObj.Value % 2 == 0,

            trueBuilder => trueBuilder
                .Then<WaitEventState>()
                .Input(s => s.Event, TestItemWaitEvent)
                .Output(wfObj => wfObj.EventValue, s => (int)s.ReceivedData!)
                .Then(_ => { }));
    }
}