using Anch.Workflow.Definition;

namespace Anch.Workflow;

public interface IWorkflow
{
    IWorkflowDefinition Definition { get; }
}

public interface IWorkflow<out TSource> : IWorkflow;