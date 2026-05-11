using Anch.Core;
using Anch.Core.DictionaryCache;
using Anch.Workflow.Domain.Definition;
using Anch.Workflow.Domain.Runtime;
using Anch.Workflow.Persistence;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.Workflow.Engine;

public class WorkflowMachineFactory(
    IServiceProxyFactory serviceProxyFactory,
    [FromKeyedServices(IWorkflowRepositoryFactory.CacheKey)]
    IWorkflowRepositoryFactory workflowRepositoryFactory) : IWorkflowMachineFactory
{
    private readonly IDictionaryCache<WorkflowInstance, IWorkflowMachine> cache =

        new DictionaryCache<WorkflowInstance, IWorkflowMachine>(wi =>

            serviceProxyFactory.Create<WorkflowMachine>(workflowRepositoryFactory.Create(wi.Definition.Identity), wi));

    public IWorkflowMachine Create(WorkflowInstance workflowInstance) => this.cache[workflowInstance];

    public IWorkflowMachine Create<TSource>(TSource source, IWorkflowDefinition<TSource> workflowDefinition)
        where TSource : class
    {
        var wi = new WorkflowInstance
        {
            Definition = workflowDefinition,
            Source = source,
            Status = WorkflowStatus.NotStarted
        };

        var m = this.Create(wi);

        m.SetStartState();

        return m;
    }
}