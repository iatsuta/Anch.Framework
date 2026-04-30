using Anch.Workflow.Domain.Runtime;
using Anch.Workflow.States._Base;

namespace Anch.Workflow.Engine;

public interface ICodeStateResolver
{
    IState Resolve(StateInstance stateInstance);
}