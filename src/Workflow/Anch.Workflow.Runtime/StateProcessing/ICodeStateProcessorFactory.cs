using Anch.Workflow.Domain.Runtime;

namespace Anch.Workflow.StateProcessing;

public interface ICodeStateProcessorFactory
{
    ICodeStateProcessor Create(StateInstance stateInstance);
}