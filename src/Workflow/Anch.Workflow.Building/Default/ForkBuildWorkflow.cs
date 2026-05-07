using Anch.Workflow.Building.Default.DomainDefinition;

namespace Anch.Workflow.Building.Default;

public class ForkBuildWorkflow<TSource, TStatus>(
    Action<IWorkflowBuilder<TSource, TStatus>> setupBuilder,
    WorkflowDefinitionBuilder<TSource, TStatus> parentWorkflowDefinitionBuilder) : ActionBuildWorkflow<TSource, TStatus>(setupBuilder)
    where TSource : class
    where TStatus : struct
{
    protected override WorkflowDefinitionBuilder<TSource, TStatus> CreateDefinitionHeader()
    {
        var header = base.CreateDefinitionHeader();

        header.StatusAccessors = parentWorkflowDefinitionBuilder.StatusAccessors;

        return header;
    }
}