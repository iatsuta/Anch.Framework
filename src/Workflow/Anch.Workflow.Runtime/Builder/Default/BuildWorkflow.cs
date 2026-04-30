using Anch.Core;
using Anch.Workflow.Builder.Default.DomainDefinition;
using Anch.Workflow.Definition;

namespace Anch.Workflow.Builder.Default;

public abstract class BuildWorkflow : IWorkflow
{
    private readonly Lazy<WorkflowDefinition> lazyDefinition;

    protected BuildWorkflow()
    {
        this.lazyDefinition = LazyHelper.Create(this.CreateDefinition);
    }

    public IWorkflowDefinition Definition => this.BaseDefinition;

    public WorkflowDefinition BaseDefinition => this.lazyDefinition.Value;

    protected abstract WorkflowDefinition CreateDefinition();

    public override string ToString()
    {
        return $"Workflow ({this.Definition.Identity.Name})";
    }
}

public abstract class BuildWorkflow<TSource> : BuildWorkflow, IWorkflow<TSource>
    where TSource : notnull
{
    protected abstract void Build(IWorkflowBuilder<TSource> builder);

    protected override WorkflowDefinition CreateDefinition()
    {
        var workflowDefinition = new WorkflowDefinition(typeof(TSource))
        {
            Identity = new(this.GetType().Name)
        };

        {
            var builder = new WorkflowBuilder<TSource>(workflowDefinition);

            this.Build(builder);
        }

        workflowDefinition.Optimize();
        workflowDefinition.Validate();
        workflowDefinition.ReplaceAutoNames();

        return workflowDefinition;
    }
}