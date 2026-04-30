using Anch.Workflow.Definition;

namespace Anch.Workflow.StateFactory;

public interface IStateFactoryCache
{
    IStateFactory GetStateFactory(IStateDefinition stateDefinition);
}