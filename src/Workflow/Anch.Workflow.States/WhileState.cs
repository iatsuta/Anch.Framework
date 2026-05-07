using Anch.Workflow.Engine;
using Anch.Workflow.Execution;

namespace Anch.Workflow.States;

public class WhileState<TLoopWorkflow, TSource>(IWorkflowHost workflowHost) : IState
    where TLoopWorkflow : IWorkflow<TSource>
    where TSource : class
{
    public bool Condition { get; set; }

    public TSource Source { get; set; } = null!;

    public TLoopWorkflow LoopWorkflow { get; set; } = default!;

    public async ValueTask<ExecutionResult> Run(IExecutionContext executionContext)
    {
        if (!this.Condition)
        {
            return new Done();
        }

        throw new NotImplementedException();

        //var startResult = await workflowHost
        //    .CreateExecutor(WorkflowExecutionPolicy.SingleStep)
        //    .StartWorkflow(this.Source, this.LoopWorkflow, executionContext.CancellationToken);

        //var wi = startResult.Modified.First();

        //wi.Owner = executionContext.StateInstance;

        //executionContext.StateInstance.Children.Add(wi);

        //if (wi.Status == WorkflowStatus.Finished)
        //{
        //    return new Done();
        //}

        //return new MultiExecutionResult(
        //[
        //    new WorkflowProcessExecutionResult(startResult, false),
        //    new WaitEventResult(EventHeader.WorkflowFinished, wi)
        //]);
    }
}