using Anch.Core.DictionaryCache;
using Anch.Workflow.Domain.Runtime;
using Anch.Workflow.StateFactory;
using Anch.Workflow.States._Base;

namespace Anch.Workflow.Engine;

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