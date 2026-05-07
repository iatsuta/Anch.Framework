using Anch.Workflow.Domain.Definition;

namespace Anch.Workflow;

public interface IWorkflow<TSource, TStatus> : IWorkflow<TSource>
    where TSource : class
    where TStatus : struct
{
    new IWorkflowDefinition<TSource, TStatus> Definition { get; }

    IWorkflowDefinition<TSource> IWorkflow<TSource>.Definition => this.Definition;
}

public interface IWorkflow<TSource> : IWorkflow
    where TSource : class
{
    new IWorkflowDefinition<TSource> Definition { get; }

    IWorkflowDefinition IWorkflow.Definition => this.Definition;
}

public interface IWorkflow
{
    IWorkflowDefinition Definition { get; }
}