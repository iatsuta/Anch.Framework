using SyncWorkflow.Engine;

namespace SyncWorkflow.States;

public class StartWorkflowsState<TSource, TElement>(IWorkflowHost host) : ParallelStateBase<TSource>(host)
    where TElement : notnull
{
    private List<TElement> elements = [];


    public IEnumerable<TElement> Elements
    {
        get => this.elements;
        set => this.elements = value.ToList();
    }


    public IWorkflow<TElement> ElementWorkflow { get; set; } = null!;


    protected override IEnumerable<IWorkflowMachine> CreateChildMachines(TSource source)
    {
        return this.Elements.Select(childrenElement => this.Host.CreateMachine(childrenElement, this.ElementWorkflow));
    }
}