using SyncWorkflow.Engine;
using SyncWorkflow.ExecutionResult;
using SyncWorkflow.States._Base;

namespace SyncWorkflow.States;

public class ForeachState<TSource, TElement>(IWorkflowHost workflowHost) : IState
{
    private List<TElement> elements = [];

    private readonly IWorkflowHost workflowHost = workflowHost;


    public IEnumerable<TElement> Elements
    {
        get => this.elements;
        set => this.elements = value.ToList();
    }

    public IWorkflow<(TSource, TElement)> ElementWorkflow { get; set; } = null!;

    public int CurrentIndex { get; set; }

    public async Task<IExecutionResult> Run(IExecutionContext executionContext)
    {
        throw new NotImplementedException();

        //var wi = await this.workflowHost.StartWorkflow(this.elements[this.CurrentIndex], this.ElementWorkflow);

        //return new WaitEventResult(new EventInfo(EventHeader.WorkflowFinished) WaitEventInfo<WorkflowInstance>(EventHeader.WorkflowFinished, wi);
    }
}
