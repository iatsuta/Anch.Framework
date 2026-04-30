using Anch.Workflow.Engine;
using Anch.Workflow.ExecutionResult;
using Anch.Workflow.States._Base;

namespace Anch.Workflow.States;

public class StartWorkflowState<TInnerSource>(IWorkflowHost host) : IState
    where TInnerSource : notnull
{
    public IWorkflow<TInnerSource> InnerWorkflow { get; set; } = null!;

    public TInnerSource InnerSource { get; set; } = default!;

    public StartWorkflowMode Mode { get; set; } = StartWorkflowMode.WaitFinish;

    public StateLeavePolicy LeavePolicy { get; set; } = StateLeavePolicy.TerminateChild;

    public async Task<IExecutionResult> Run(IExecutionContext executionContext)
    {
        if (executionContext.IsCallbackEvent)
        {
            if (executionContext.CallbackEventInfo!.Event == EventHeader.WorkflowFinished
                && executionContext.CallbackEventInfo.SourceWorkflow!.Owner == executionContext.StateInstance)
            {
                return new Done();
            }
            else
            {
                throw new InvalidOperationException("Wrong callback");
            }
        }
        else
        {
            var startResult = await host.CreateExecutor(WorkflowExecutionPolicy.Head).StartWorkflow(this.InnerSource, this.InnerWorkflow, executionContext.CancellationToken);

            var wi = startResult.Started.First().WorkflowInstance;

            wi.Owner = executionContext.StateInstance;

            executionContext.StateInstance.Child.Add(wi);

#if DEBUG
            if (wi.Status == WorkflowStatus.Finished ^ startResult.Unprocessed.Any())
            {
                throw new InvalidOperationException();
            }
#endif

            if (wi.Status == WorkflowStatus.Finished)
            {
                return new Done();
            }
            else 
            {
                switch (this.Mode)
                {
                    case StartWorkflowMode.WaitFinish:
                        return new MultiExecutionResult(
                            new WorkflowProcessExecutionResult(startResult, false),
                            new WaitEventResult(EventHeader.WorkflowFinished, wi));

                    case StartWorkflowMode.FireAndForget:
                        return new WorkflowProcessExecutionResult(startResult, true);

                    default:
                        throw new ArgumentOutOfRangeException(nameof(this.Mode));
                }
            }
        }
    }
}