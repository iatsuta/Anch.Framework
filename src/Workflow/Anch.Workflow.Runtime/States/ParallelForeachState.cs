using Anch.Workflow.Domain.Definition;
using Anch.Workflow.Engine;

namespace Anch.Workflow.States;

public class ParallelForeachState<TSource, TElement>(IWorkflowMachineFactory workflowMachineFactory) : ParallelStateBase<TSource>
{
    public IReadOnlyList<TElement> Elements { get; set; } = [];


    public IWorkflowDefinition<(TSource, TElement)> ElementWorkflow { get; set; } = null!;

    protected override IEnumerable<IWorkflowMachine> CreateChildMachines(TSource source)
    {
        return this.Elements.Select(childrenElement => workflowMachineFactory.Create((source, childrenElement), this.ElementWorkflow));
    }
}