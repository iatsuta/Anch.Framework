using SyncWorkflow.Engine;

namespace SyncWorkflow.States;

public class ParallelState<TSource>(IWorkflowHost host) : ParallelStateBase<TSource>(host)
    where TSource : notnull
{
    private List<IWorkflow<TSource>> forks = [];


    public IEnumerable<IWorkflow<TSource>> Forks
    {
        get => this.forks;
        set => this.forks = value.ToList();
    }

    protected override IEnumerable<IWorkflowMachine> CreateChildMachines(TSource source)
    {
        return this.Forks.Select(fork => this.Host.CreateMachine(source, fork));
    }
}