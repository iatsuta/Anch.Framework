using Anch.Workflow.Domain.Definition;
using Anch.Workflow.Engine;
using Anch.Workflow.Execution;

namespace Anch.Workflow.States;

public class ForeachState<TSource, TElement>(IWorkflowHost workflowHost) : IState
{
    public IReadOnlyList<TElement> Elements { get; set; } = [];

    public IWorkflowDefinition<(TSource, TElement)> ElementWorkflow { get; set; } = null!;

    public int CurrentIndex { get; set; }

    public async ValueTask<ExecutionResult> Run(IExecutionContext executionContext)
    {
        throw new NotImplementedException();

        //if (executionContext.IsCallbackEvent
        //    && executionContext.CallbackEventInfo!.Header == EventHeader.WorkflowFinished)
        //{
        //    this.CurrentIndex++;
        //}

        //if (this.CurrentIndex >= this.elements.Count)
        //{
        //    return new Done();
        //}

        //var startResult = await workflowHost
        //    .CreateExecutor(WorkflowExecutionPolicy.SingleStep)
        //    .StartWorkflow(
        //        ((TSource)executionContext.Source, this.elements[this.CurrentIndex]),
        //        this.ElementWorkflow,
        //        executionContext.CancellationToken);

        //var wi = startResult.Modified.First();

        //wi.Owner = executionContext.StateInstance;

        //executionContext.StateInstance.Children.Add(wi);

        //if (wi.Status == WorkflowStatus.Finished)
        //{
        //    this.CurrentIndex++;

        //    return await this.Run(executionContext); <--- FIX
        //}

        //return new MultiExecutionResult(
        //[
        //    new WorkflowProcessExecutionResult(startResult, false),
        //    new WaitEventResult(EventHeader.WorkflowFinished, wi)
        //]);
    }
}