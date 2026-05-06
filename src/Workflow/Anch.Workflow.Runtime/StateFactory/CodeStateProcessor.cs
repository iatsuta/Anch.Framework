using Anch.Core;
using Anch.Workflow.Domain.Definition;
using Anch.Workflow.States;

namespace Anch.Workflow.StateFactory;

public class CodeStateProcessor<TSource, TStatus, TState>(
    IServiceProxyFactory serviceProxyFactory,
    IServiceProvider serviceProvider,
    IStateDefinition<TSource, TStatus, TState> stateDefinition,
    TSource source) : ICodeStateProcessor
    where TSource : notnull
    where TStatus : struct
    where TState : IState
{
    private readonly TState codeState = serviceProxyFactory.Create<TState>();

    public IState CodeState => this.codeState;

    public void SetStatus()
    {
        if (stateDefinition is { Status: { } status, Workflow.StatusAccessors: { } statusAccessor })
        {
            statusAccessor.Setter(source, status);
        }
    }

    public async ValueTask BindInput(CancellationToken ct)
    {
        foreach (var initAction in stateDefinition.InputActions)
        {
            await initAction(serviceProvider, source, this.codeState, ct);
        }
    }

    public async ValueTask BindOutput(CancellationToken ct)
    {
        foreach (var outputAction in stateDefinition.OutputActions)
        {
            await outputAction(serviceProvider, this.codeState, source, ct);
        }
    }
}