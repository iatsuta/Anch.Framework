using Anch.Core;
using Anch.Workflow.Definition;
using Anch.Workflow.Domain.Runtime;
using Anch.Workflow.ExecutionResult;
using Anch.Workflow.StateFactory;
using Anch.Workflow.Storage;

namespace Anch.Workflow.Engine;

public class WorkflowMachine<TSource>(
    IServiceProvider serviceProvider,
    IWorkflowHost host,
    IStateFactoryCache stateFactoryCache,
    ISpecificWorkflowStorage storage,
    WorkflowInstance workflowInstance,
    ICodeStateResolver codeStateResolver,
    IWorkflowEventListener? eventListener = null)
    : IWorkflowMachine
{
    private TSource Source => (TSource)this.WorkflowInstance.Source;

    public WorkflowInstance WorkflowInstance { get; } = workflowInstance;

    public void SetStartState()
    {
        this.SetCurrentState(this.WorkflowInstance.Definition.StartState);
    }

    public async Task Save(CancellationToken cancellationToken)
    {
        await storage.SaveWorkflowInstance(this.WorkflowInstance, cancellationToken);
    }

    public async Task<WorkflowProcessResult> ProcessWorkflow(CancellationToken cancellationToken)
    {
        return await this.ProcessCurrentState(this.CreateExecutionContext(cancellationToken));
    }

    public async Task<WorkflowProcessResult> Terminate(CancellationToken cancellationToken)
    {
        if (this.WorkflowInstance.Status.Role != WorkflowStatusRole.Finished)
        {
            return await this.SwitchState(this.WorkflowInstance.Definition.TerminateState, cancellationToken);
        }
        else
        {
            return WorkflowProcessResult.Empty;
        }
    }

    protected virtual IExecutionContext CreateExecutionContext(CancellationToken cancellationToken, WaitEventInfo? callbackEventInfo = null)
    {
        return this.CreateExecutionContext(
            this.WorkflowInstance.CurrentState,
            cancellationToken,
            callbackEventInfo);
    }

    protected virtual IExecutionContext CreateExecutionContext(StateInstance stateInstance, CancellationToken cancellationToken, WaitEventInfo? callbackEventInfo = null)
    {
        return new ExecutionContext
        {
            StateInstance = stateInstance,
            CancellationToken = cancellationToken,
            CallbackEventInfo = callbackEventInfo
        };
    }

    private void SetCurrentState(IStateDefinition stateDefinition)
    {
        this.WorkflowInstance.CurrentState = new StateInstance
        {
            Workflow = this.WorkflowInstance,
            Definition = stateDefinition,
        };

        eventListener?.OnCurrentStateChanged(this.WorkflowInstance);
    }

    private async Task<WorkflowProcessResult> SwitchState(IStateDefinition newStateDefinition, CancellationToken cancellationToken)
    {
        this.SetCurrentState(newStateDefinition);

        return await this.ProcessCurrentState(this.CreateExecutionContext(cancellationToken));
    }

    private async Task<WorkflowProcessResult> ProcessCurrentState(IExecutionContext executionContext)
    {
        await this.Save(executionContext.CancellationToken);

        if (!executionContext.StateInstance.IsActual)
        {
            return WorkflowProcessResult.Empty;
        }

        executionContext.WorkflowInstance.SetStatus(WorkflowStatus.Runnable);

        var currentState = executionContext.StateInstance;
        var codeState = codeStateResolver.Resolve(currentState);

        if (!currentState.InputProcessed)
        {
            currentState.InputProcessed = true;

            await stateFactoryCache.GetStateFactory(currentState.Definition).BindInput(codeState, serviceProvider, executionContext.Source, executionContext.CancellationToken);
        }

        return new WorkflowProcessResult(async () =>
        {
            var executionResult = await codeState.Run(executionContext);

            if (!currentState.OutputProcessed && executionResult.LeaveState)
            {
                currentState.OutputProcessed = true;

                await stateFactoryCache.GetStateFactory(currentState.Definition)
                    .BindOutput(codeState, serviceProvider, executionContext.Source, executionContext.CancellationToken);

                var leaveResult = await codeState.LeavePolicy.Leave(serviceProvider, executionContext);
                
                return leaveResult + new WorkflowProcessResult(() => this.ProcessExecutionResult(executionContext, executionResult));
            }
            else
            {
                return await this.ProcessExecutionResult(executionContext, executionResult);
            }
        });
    }

    protected virtual async Task<WorkflowProcessResult> ProcessExecutionResult(IExecutionContext executionContext, IExecutionResult executionResult)
    {
        switch (executionResult)
        {
            case Wait:
                executionContext.WorkflowInstance.SetStatus(WorkflowStatus.WaitEvent);
                return WorkflowProcessResult.Empty;

            case WaitEventResult waitEventResult:
                executionContext.WorkflowInstance.SetStatus(WorkflowStatus.WaitEvent);

                executionContext.StateInstance.RegisterWaitEvent(waitEventResult.ToEventInfo(executionContext.StateInstance));
                return WorkflowProcessResult.Empty;

            case Done:
                return await this.ProcessExecutionResult(executionContext, new PushEventResult(EventHeader.StateDone, executionContext.StateInstance));

            case PushEventResult pushEventResult:
                return await this.ProcessExecutionResult(executionContext, pushEventResult);

            case MultiExecutionResult multiExecutionResult:
                return await multiExecutionResult.ExecutionResults
                    .Select(subExecutionResult => this.ProcessExecutionResult(executionContext, subExecutionResult))
                    .Aggregate();

            default:
                throw new ArgumentOutOfRangeException(nameof(executionResult));
        }
    }

    private async Task<WorkflowProcessResult> ProcessExecutionResult(IExecutionContext executionContext, PushEventResult pushEventResult)
    {
        return await this.ProcessExecutionResult(executionContext, pushEventResult.ToEventInfo(executionContext.WorkflowInstance));
    }

    private async Task<WorkflowProcessResult> ProcessExecutionResult(IExecutionContext executionContext, PushEventInfo pushEventInfo)
    {
        if (pushEventInfo.TargetState == null && pushEventInfo.Event.IsGlobal)
        {
            if (pushEventInfo.Event == EventHeader.WorkflowFinished)
            {
                if (executionContext.WorkflowInstance.Status.Role != WorkflowStatusRole.Finished)
                {
                    executionContext.WorkflowInstance.SetStatus(WorkflowStatus.Finished);
                }
            }
            else if (pushEventInfo.Event == EventHeader.WorkflowTerminated)
            {
                executionContext.WorkflowInstance.SetStatus(WorkflowStatus.Terminated);
            }

            return await host.CreateExecutor(WorkflowExecutionPolicy.Head).PushEvent(pushEventInfo, executionContext.CancellationToken);
        }
        else if (pushEventInfo.TargetState.Maybe(s => s.Workflow != executionContext.WorkflowInstance))
        {
            return await host.CreateExecutor(WorkflowExecutionPolicy.Head).PushEvent(pushEventInfo, executionContext.CancellationToken);
        }
        else
        {
            var transition = executionContext.StateInstance.Definition.Transitions.Single(tr => tr.Event.Header == pushEventInfo.Event);

            return await this.SwitchState(transition.To, executionContext.CancellationToken);
        }
    }

    public async Task<WorkflowProcessResult> PushReleasedEvent(WaitEventInfo releasedEventInfo, CancellationToken cancellationToken)
    {
        var executionContext = this.CreateExecutionContext(releasedEventInfo.TargetState, cancellationToken, releasedEventInfo);

        return await this.ProcessCurrentState(executionContext);
    }
}