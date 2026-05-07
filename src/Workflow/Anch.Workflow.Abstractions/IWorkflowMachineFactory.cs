using Anch.Workflow.Domain.Definition;
using Anch.Workflow.Domain.Runtime;

namespace Anch.Workflow;

public interface IWorkflowMachineFactory
{
    IWorkflowMachine Create(WorkflowInstance wi);

    IWorkflowMachine Create<TSource>(TSource source, IWorkflowDefinition<TSource> workflowDefinition)
        where TSource : class;
}