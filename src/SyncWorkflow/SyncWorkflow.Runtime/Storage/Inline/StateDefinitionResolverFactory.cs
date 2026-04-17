using Microsoft.Extensions.DependencyInjection;

namespace SyncWorkflow.Storage.Inline;

public class StateDefinitionResolverFactory(IServiceProvider serviceProvider) : IStateDefinitionResolverFactory
{
    public IStateDefinitionResolver<TSource> Create<TSource>(IWorkflow<TSource> workflow)
    {
        var statusType = workflow.Definition.StatusProperty!.Body.Type;

        var stateDefinitionResolverType = typeof(StateDefinitionResolver<,>).MakeGenericType(typeof(TSource), statusType);

        return (IStateDefinitionResolver<TSource>)ActivatorUtilities.CreateInstance(serviceProvider, stateDefinitionResolverType, workflow);
    }
}