using Anch.Core;
using Anch.Core.DictionaryCache;
using Anch.Workflow.Domain.Runtime;

namespace Anch.Workflow.StateProcessing;

public class CodeStateProcessorFactory(IServiceProxyFactory serviceProxyFactory) : ICodeStateProcessorFactory
{
    private readonly IDictionaryCache<StateInstance, ICodeStateProcessor> cache =
        new DictionaryCache<StateInstance, ICodeStateProcessor>(si =>
            serviceProxyFactory.Create<ICodeStateProcessor>(
                typeof(CodeStateProcessor<,,>)
                    .MakeGenericType(si.Workflow.Definition.SourceType, si.Workflow.Definition.StatusType, si.Definition.StateType), si.Definition,
                si.Workflow.Source));

    public ICodeStateProcessor Create(StateInstance stateInstance) => this.cache[stateInstance];
}