using Anch.Workflow.Engine;

namespace Anch.Workflow.States;

public class StartWorkflowsState<TSource, TElement>(IWorkflowMachineFactory workflowMachineFactory) : ParallelStateBase<TSource>
    where TElement : notnull
{
    public IReadOnlyList<TElement> Elements { get; set; } = [];


    public IWorkflow<TElement> ElementWorkflow { get; set; } = null!;


    protected override IEnumerable<IWorkflowMachine> CreateChildMachines(TSource source)
    {
        return this.Elements.Select(childrenElement => workflowMachineFactory.Create(childrenElement, this.ElementWorkflow.Definition));
    }
}