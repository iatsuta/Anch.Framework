using SyncWorkflow.Domain.Definition;

namespace SyncWorkflow.StateFactory;

public interface IStateFactoryCache
{
    IStateFactory GetStateFactory(IStateDefinition stateDefinition);
}