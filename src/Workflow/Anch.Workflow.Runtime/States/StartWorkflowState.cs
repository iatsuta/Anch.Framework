using Anch.Workflow.Domain;
using Anch.Workflow.Domain.Definition;
using Anch.Workflow.Domain.Runtime;
using Anch.Workflow.Engine;
using Anch.Workflow.Execution;

namespace Anch.Workflow.States;

public class StartWorkflowState<TInnerSource>(IWorkflowExecutor workflowExecutor) : IState
    where TInnerSource : notnull
{
    public IWorkflowDefinition<TInnerSource> InnerWorkflow { get; set; } = null!;

    public TInnerSource InnerSource { get; set; } = default!;

    public StartWorkflowMode Mode { get; set; } = StartWorkflowMode.WaitFinish;

    public StateLeavePolicy LeavePolicy { get; set; } = StateLeavePolicy.TerminateChild;

    public async ValueTask<ExecutionResult> Run(IExecutionContext executionContext)
    {
        if (executionContext.CallbackEventInfo is { } callbackEventInfo)
        {
            if (callbackEventInfo.Header == EventHeader.WorkflowFinished
                && callbackEventInfo.SourceWorkflow?.Owner == executionContext.StateInstance)
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
            var startResult = await workflowExecutor.Start(this.InnerSource, this.InnerWorkflow, executionContext.CancellationToken);

            var wi = startResult.Modified.First();

            wi.Owner = executionContext.StateInstance;

            executionContext.StateInstance.Children.Add(wi);

#if DEBUG
            if (wi.Status == WorkflowStatus.Finished != startResult.Unprocessed.IsEmpty)
            {
                throw new InvalidOperationException();
            }
#endif

            if (wi.Status == WorkflowStatus.Finished || this.Mode == StartWorkflowMode.FireAndForget)
            {
                return new Done { WorkflowProcessResult = startResult };
            }
            else
            {
                return new WaitEventResult(EventHeader.WorkflowFinished, wi)
                {
                    WorkflowProcessResult = startResult
                };
            }
        }
    }
}