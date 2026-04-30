using Anch.Workflow.Engine;
using Anch.Workflow.ExecutionResult;
using Anch.Workflow.States._Base;

namespace Anch.Workflow.States;

public class WhileState<TLoopWorkflow, TSource>(IWorkflowHost workflowHost) : IState
    where TLoopWorkflow : IWorkflow<TSource>
{
    private readonly IWorkflowHost workflowHost = workflowHost;


    public bool Condition { get; set; }

    public TSource Source { get; set; } = default!;

    public TLoopWorkflow LoopWorkflow { get; set; } = default!;

    public async Task<IExecutionResult> Run(IExecutionContext executionContext)
    {
        throw new NotImplementedException();
        //if (this.Condition)
        //{
        //    return new Done();
        //}
        //else
        //{
        //    var wi = await this.workflowHost.StartWorkflow(this.Source, this.LoopWorkflow);

        //    return new WaitEventInfo<WorkflowInstance>(EventHeader.WorkflowFinished, wi);
        //}
    }
}