using Anch.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.Workflow.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWorkflow(this IServiceCollection services, Action<IWorkflowSetup> setupAction) =>
        services.Initialize<WorkflowSetup>(setupAction);
}