using Microsoft.Extensions.DependencyInjection;

using SyncWorkflow.Domain.Runtime;
using SyncWorkflow.Engine;
using SyncWorkflow.Storage;

namespace SyncWorkflow.Tests;

public record InlineScope(IServiceProvider ServiceProvider)
{
    public IWorkflowStorage Storage => this.ServiceProvider.GetRequiredService<IWorkflowStorage>();

    public IWorkflowHost Host => this.ServiceProvider.GetWorkflowHost();

    public async Task<WorkflowInstance> StartWorkflow<TSource, TWorkflow>(TSource source, CancellationToken cancellationToken = default)
        where TWorkflow : IWorkflow<TSource>, new()
    {
        return await this.Host.StartWorkflow<TSource, TWorkflow>(source, cancellationToken);
    }
}