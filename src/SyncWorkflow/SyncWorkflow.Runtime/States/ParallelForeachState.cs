using SyncWorkflow.Engine;

namespace SyncWorkflow.States;

public class ParallelForeachState<TSource, TElement>(IWorkflowHost host) : ParallelStateBase<TSource>(host)
{
    private List<TElement> elements = [];


    public IEnumerable<TElement> Elements
    {
        get => this.elements;
        set => this.elements = value.ToList();
    }


    public IWorkflow<(TSource, TElement)> ElementWorkflow { get; set; } = null!;

    protected override IEnumerable<IWorkflowMachine> CreateChildMachines(TSource source)
    {
        return this.Elements.Select(childrenElement => this.Host.CreateMachine((source, childrenElement), this.ElementWorkflow));
    }
}