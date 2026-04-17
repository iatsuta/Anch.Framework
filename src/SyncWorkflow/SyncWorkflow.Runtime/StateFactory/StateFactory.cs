using SyncWorkflow.Domain.Definition;
using SyncWorkflow.States;
using SyncWorkflow.States._Base;

namespace SyncWorkflow.StateFactory;

public class StateFactory<TState, TSource>(IStateDefinition stateDefinition) : IStateFactory
    where TState : IState
{
    private readonly List<Func<IServiceProvider, TSource, TState, CancellationToken, Task>> inputActions =
        stateDefinition.InputActions.Cast<Func<IServiceProvider, TSource, TState, CancellationToken, Task>>().ToList();

    private readonly List<Func<IServiceProvider, TState, TSource, CancellationToken, Task>> outputActions =
        stateDefinition.OutputActions.Cast<Func<IServiceProvider, TState, TSource, CancellationToken, Task>>().ToList();

    public IState CreateState(IServiceProvider serviceProvider, object source)
    {
        return ActivatorUtilities.CreateInstance<TState>(serviceProvider);
    }

    public async Task BindInput(IState state, IServiceProvider serviceProvider, object source, CancellationToken ct)
    {
        foreach (var initAction in this.inputActions)
        {
            await initAction(serviceProvider, (TSource)source, (TState)state, ct);
        }
    }

    public async Task BindOutput(IState state, IServiceProvider serviceProvider, object source, CancellationToken ct)
    {
        foreach (var outputAction in this.outputActions)
        {
            await outputAction(serviceProvider, (TState)state, (TSource)source, ct);
        }
    }
}