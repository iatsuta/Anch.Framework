using Anch.Workflow.Engine;
using Anch.Workflow.Storage;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.Workflow.Tests._Base;

public static class ServiceProviderExtensions
{
    public static IWorkflowHost GetWorkflowHost(this IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<IWorkflowHost>();
    }

    public static IWorkflowStorage GetWorkflowStorage(this IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<IWorkflowStorage>();
    }
}