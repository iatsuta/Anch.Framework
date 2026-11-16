using Microsoft.Extensions.DependencyInjection;

namespace Anch.Workflow.Persistence;

public interface IWorkflowDatabaseProvider
{
    void AddServices(IServiceCollection services);
}