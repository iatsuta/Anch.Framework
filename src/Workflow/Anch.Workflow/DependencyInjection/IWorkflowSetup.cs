using Anch.Workflow.Persistence;

namespace Anch.Workflow.DependencyInjection;

public interface IWorkflowSetup
{
    IWorkflowSetup SetDatabaseProvider<TWorkflowDatabaseProvider>()
        where TWorkflowDatabaseProvider : IWorkflowDatabaseProvider, new();

    IWorkflowSetup Add<TWorkflow>()
        where TWorkflow : class, IWorkflow;
}