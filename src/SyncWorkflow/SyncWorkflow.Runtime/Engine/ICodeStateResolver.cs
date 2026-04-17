using SyncWorkflow.Domain.Runtime;
using SyncWorkflow.States;
using SyncWorkflow.States._Base;

namespace SyncWorkflow.Engine;

public interface ICodeStateResolver
{
    IState Resolve(StateInstance stateInstance);
}