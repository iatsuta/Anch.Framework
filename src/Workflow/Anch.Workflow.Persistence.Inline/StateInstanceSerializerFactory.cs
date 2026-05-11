using Anch.Core;
using Anch.Workflow.Domain.Definition;

namespace Anch.Workflow.Persistence.Inline;

public class StateInstanceSerializerFactory<TSource, TStatus>(IServiceProxyFactory serviceProxyFactory)
    : IStateInstanceSerializerFactory<TSource, TStatus>
    where TSource : class
    where TStatus : struct
{
    public IStateInstanceSerializer Create(IWorkflowDefinition<TSource, TStatus> workflowDefinition) =>
        serviceProxyFactory.Create<StateInstanceSerializer<TSource, TStatus>>(workflowDefinition);
}