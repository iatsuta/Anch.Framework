namespace Anch.Workflow.DependencyInjection;

public interface IWorkflowSetup
{
    IWorkflowSetup Add<TWorkflow>()
        where TWorkflow : class, IWorkflow;
}