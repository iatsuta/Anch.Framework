using Anch.Workflow.Domain.Definition;

namespace Anch.Workflow.States;

public class ParallelState<TSource>(IWorkflowMachineFactory workflowMachineFactory) : ParallelStateBase<TSource>
    where TSource : class
{
    public IReadOnlyList<IWorkflowDefinition<TSource>> Forks { get; set; } = [];

    protected override IEnumerable<IWorkflowMachine> CreateChildMachines(TSource source)
    {
        return this.Forks.Select(fork => workflowMachineFactory.Create(source, fork));
    }
}