using Anch.Workflow.Builder.Default.DomainDefinition;

namespace Anch.Workflow.Builder.Default;

public class ActionBuildWorkflow<TSource, TStatus>(Action<IWorkflowBuilder<TSource, TStatus>> setupBuilder) : BuildWorkflow<TSource, TStatus>
    where TSource : notnull
    where TStatus : struct
{
    protected override WorkflowDefinitionBuilder<TSource, TStatus> CreateDefinitionHeader()
    {
        var header = base.CreateDefinitionHeader();

        header.IsRoot = false;
        header.AllowReplaceAutoNames = false;

        return header;
    }

    protected sealed override void Build(IWorkflowBuilder<TSource, TStatus> builder) => setupBuilder(builder);
}