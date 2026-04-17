using CommonFramework.DictionaryCache;
using SyncWorkflow.Domain.Runtime;
using SyncWorkflow.StateFactory;
using SyncWorkflow.States;
using SyncWorkflow.States._Base;

namespace SyncWorkflow.Engine;

public class CodeStateResolver(IServiceProvider serviceProvider, IStateFactoryCache stateFactoryCache) : ICodeStateResolver
{
    private readonly IDictionaryCache<StateInstance, IState> stateCache = new DictionaryCache<StateInstance, IState>(stateInstance =>
        stateFactoryCache
            .GetStateFactory(stateInstance.Definition)
            .CreateState(serviceProvider, stateInstance.Workflow.Source));

    public IState Resolve(StateInstance stateInstance)
    {
        return this.stateCache[stateInstance];
    }
}