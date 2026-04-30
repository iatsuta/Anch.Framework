using Microsoft.Extensions.DependencyInjection;

namespace Anch.Workflow.Storage.Inline;

public class StateInstanceSerializerFactory(IServiceProvider serviceProvider) : IStateInstanceSerializerFactory
{
    public IStateInstanceSerializer<TSource> Create<TSource>(IWorkflow<TSource> workflow)
    {
        return ActivatorUtilities.CreateInstance<StateInstanceSerializer<TSource>>(serviceProvider, workflow);
    }
}