using Anch.Workflow.Domain.Definition;
using Anch.Workflow.Engine;

namespace Anch.Workflow.States;

public class ParallelState<TSource>(IWorkflowMachineFactory workflowMachineFactory) : ParallelStateBase<TSource>
    where TSource : notnull
{
    public IReadOnlyList<IWorkflowDefinition<TSource>> Forks { get; set; } = [];

    protected override IEnumerable<IWorkflowMachine> CreateChildMachines(TSource source)
    {
        return this.Forks.Select(fork => workflowMachineFactory.Create(source, fork));
    }
}