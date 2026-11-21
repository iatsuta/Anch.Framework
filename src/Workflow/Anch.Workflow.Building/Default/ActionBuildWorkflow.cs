using Anch.Workflow.Building.Default.DomainDefinition;

namespace Anch.Workflow.Building.Default;

public class ActionBuildWorkflow<TSource, TStatus>(Action<IWorkflowBuilder<TSource, TStatus>> setupBuilder) : BuildWorkflow<TSource, TStatus>
    where TSource : class
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