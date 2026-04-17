using SyncWorkflow.States;
using SyncWorkflow.States._Base;

namespace SyncWorkflow.StateFactory;

public interface IStateFactory
{
    IState CreateState(IServiceProvider serviceProvider, object source);

    Task BindInput(IState state, IServiceProvider serviceProvider, object source, CancellationToken ct);

    Task BindOutput(IState state, IServiceProvider serviceProvider, object source, CancellationToken ct);
}