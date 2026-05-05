namespace Anch.Workflow.Builder.Default;

public class ActionBuildWorkflow<TSource, TStatus>(Action<IWorkflowBuilder<TSource, TStatus>> setupBuilder) : BuildWorkflow<TSource, TStatus>
    where TSource : notnull
    where TStatus : notnull
{
    protected override void Build(IWorkflowBuilder<TSource, TStatus> builder) => setupBuilder(builder);
}