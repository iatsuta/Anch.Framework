using CommonFramework;
using CommonFramework.DictionaryCache;
using SyncWorkflow.Domain.Definition;

namespace SyncWorkflow.StateFactory;

public class StateFactoryCache : IStateFactoryCache
{
    private readonly IDictionaryCache<IStateDefinition, IStateFactory> cache = new DictionaryCache<IStateDefinition, IStateFactory>(
        stateDefinition => typeof(StateFactory<,>)
                          .MakeGenericType(stateDefinition.StateType, stateDefinition.Workflow.SourceType)
                          .GetConstructors()
                          .Single()
                          .Invoke([stateDefinition])
                          .Pipe(res => (IStateFactory)res))
        .WithLock();


    public IStateFactory GetStateFactory(IStateDefinition stateDefinition)
    {
        return this.cache[stateDefinition];
    }
}