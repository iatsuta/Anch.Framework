using Anch.Workflow.Domain.Definition;

namespace Anch.Workflow.States;

public class ParallelForeachState<TSource, TElement>(IWorkflowMachineFactory workflowMachineFactory) : ParallelStateBase<TSource>
{
    public IReadOnlyList<TElement> Elements { get; set; } = [];


    public IWorkflowDefinition<SourceItem<TSource, TElement>> ElementWorkflow { get; set; } = null!;

    protected override IEnumerable<IWorkflowMachine> CreateChildMachines(TSource source)
    {
        return this.Elements.Select(childrenElement => workflowMachineFactory.Create(new (source, childrenElement), this.ElementWorkflow));
    }
}