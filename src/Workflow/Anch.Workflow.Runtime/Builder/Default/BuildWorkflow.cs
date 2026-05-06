using Anch.Core;
using Anch.Workflow.Builder.Default.DomainDefinition;
using Anch.Workflow.Domain.Definition;

namespace Anch.Workflow.Builder.Default;

public abstract class BuildWorkflow<TSource> : BuildWorkflow<TSource, Ignore>
    where TSource : notnull;

public abstract class BuildWorkflow<TSource, TStatus> : IWorkflow<TSource, TStatus>
    where TSource : notnull
    where TStatus : struct
{
    protected abstract void Build(IWorkflowBuilder<TSource, TStatus> builder);


    private readonly Lazy<WorkflowDefinitionBuilder<TSource, TStatus>> lazyDefinition;

    protected BuildWorkflow() => this.lazyDefinition = LazyHelper.Create(this.CreateDefinition);

    public WorkflowDefinitionBuilder<TSource, TStatus> Definition => this.lazyDefinition.Value;

    protected virtual WorkflowDefinitionBuilder<TSource, TStatus> CreateDefinitionHeader() =>
        new() { Identity = new(this.GetType().Name), IsRoot = true, AllowReplaceAutoNames = true };

    private WorkflowDefinitionBuilder<TSource, TStatus> CreateDefinition()
    {
        var workflowDefinition = this.CreateDefinitionHeader();

        this.Build(new WorkflowBuilder<TSource, TStatus>(workflowDefinition));

        workflowDefinition.Optimize();
        workflowDefinition.Validate();

        if (workflowDefinition.AllowReplaceAutoNames)
        {
            workflowDefinition.ReplaceAutoNames();
        }

        return workflowDefinition;
    }

    public override string ToString()
    {
        return $"Workflow ({this.Definition.Identity.Name})";
    }

    IWorkflowDefinition<TSource, TStatus> IWorkflow<TSource, TStatus>.Definition => this.Definition;
}