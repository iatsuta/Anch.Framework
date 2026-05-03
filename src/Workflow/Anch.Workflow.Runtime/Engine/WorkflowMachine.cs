using System.ComponentModel.Design;

using Anch.Core;
using Anch.Workflow.Domain;
using Anch.Workflow.Domain.Definition;
using Anch.Workflow.Domain.Runtime;
using Anch.Workflow.Execution;
using Anch.Workflow.Serialization;
using Anch.Workflow.StateFactory;

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

    public Task<WorkflowProcessResult> ProcessWorkflow(IExecutionResult executionResult, CancellationToken cancellationToken) =>
        this.ProcessExecutionResult(this.WorkflowInstance.CurrentState, executionResult, cancellationToken);

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

    protected virtual IExecutionContext CreateExecutionContext(StateInstance stateInstance, CancellationToken cancellationToken,
        WaitEventInfo? callbackEventInfo = null)
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
        if (!executionContext.StateInstance.IsActual)
        {
            return WorkflowProcessResult.Empty;
        }

        await this.Save(executionContext.CancellationToken);

        executionContext.WorkflowInstance.SetStatus(WorkflowStatus.Runnable);

        var currentState = executionContext.StateInstance;
        var codeState = codeStateResolver.Resolve(currentState);

        if (!currentState.InputProcessed)
        {
            currentState.InputProcessed = true;

            await stateFactoryCache.GetStateFactory(currentState.Definition)
                .BindInput(codeState, serviceProvider, executionContext.Source, executionContext.CancellationToken);
        }

        var executionResult = await codeState.Run(executionContext);

        var workflowProcessResult = new WorkflowProcessResult([executionContext.WorkflowInstance], [new UnprocessedStateResult(currentState, executionResult)]);

        if (!currentState.OutputProcessed && executionResult.LeaveState)
        {
            currentState.OutputProcessed = true;

            await stateFactoryCache.GetStateFactory(currentState.Definition)
                .BindOutput(codeState, serviceProvider, executionContext.Source, executionContext.CancellationToken);

            var leaveResult = await codeState.LeavePolicy.Leave(serviceProvider, executionContext);

            return workflowProcessResult + leaveResult;
        }
        else
        {
            return workflowProcessResult;
        }
    }

    protected virtual async Task<WorkflowProcessResult> ProcessExecutionResult(StateInstance stateInstance, IExecutionResult executionResult,
        CancellationToken cancellationToken)
    {
        switch (executionResult)
        {
            case WorkflowProcessExecutionResult workflowProcessExecutionResult:
            {
                if (workflowProcessExecutionResult.LeaveState)
                {
                    return workflowProcessExecutionResult.WorkflowProcessResult +

                           await this.ProcessExecutionResult(stateInstance, new PushEventResult(EventHeader.StateDone, stateInstance), cancellationToken);
                }
                else
                {
                    return workflowProcessExecutionResult.WorkflowProcessResult;
                }
            }

            case Wait:
                stateInstance.Workflow.SetStatus(WorkflowStatus.WaitEvent);
                return WorkflowProcessResult.Empty;

            case WaitEventResult waitEventResult:
                stateInstance.Workflow.SetStatus(WorkflowStatus.WaitEvent);
                stateInstance.RegisterWaitEvent(waitEventResult.ToEventInfo(stateInstance));
                return WorkflowProcessResult.Empty;

            case Done:
                return await this.ProcessExecutionResult(stateInstance, new PushEventResult(EventHeader.StateDone, stateInstance), cancellationToken);

            case PushEventResult pushEventResult:
                return await this.ProcessExecutionResult(stateInstance, pushEventResult, cancellationToken);

            case MultiExecutionResult multiExecutionResult:
                return new WorkflowProcessResult([],
                    [.. multiExecutionResult.ExecutionResults.Select(er => new UnprocessedStateResult(stateInstance, er))]);

            default:
                throw new ArgumentOutOfRangeException(nameof(executionResult));
        }
    }

    private async Task<WorkflowProcessResult> ProcessExecutionResult(StateInstance stateInstance, PushEventResult pushEventResult,
        CancellationToken cancellationToken)
    {
        return await this.ProcessExecutionResult(stateInstance, pushEventResult.ToEventInfo(stateInstance.Workflow), cancellationToken);
    }

    private async Task<WorkflowProcessResult> ProcessExecutionResult(StateInstance stateInstance, PushEventInfo pushEventInfo,
        CancellationToken cancellationToken)
    {
        if (pushEventInfo.TargetState == null && pushEventInfo.Header.IsGlobal)
        {
            if (pushEventInfo.Header == EventHeader.WorkflowFinished)
            {
                if (stateInstance.Workflow.Status.Role != WorkflowStatusRole.Finished)
                {
                    stateInstance.Workflow.SetStatus(WorkflowStatus.Finished);
                }
            }
            else if (pushEventInfo.Header == EventHeader.WorkflowTerminated)
            {
                stateInstance.Workflow.SetStatus(WorkflowStatus.Terminated);
            }

            return await host.CreateExecutor(WorkflowExecutionPolicy.SingleStep).PushEvent(pushEventInfo, cancellationToken);
        }
        else if (pushEventInfo.TargetState.Maybe(s => s.Workflow != stateInstance.Workflow))
        {
            return await host.CreateExecutor(WorkflowExecutionPolicy.SingleStep).PushEvent(pushEventInfo, cancellationToken);
        }
        else
        {
            var transition = stateInstance.Definition.Transitions.Single(tr => tr.Event.Header == pushEventInfo.Header);

            return await this.SwitchState(transition.To, cancellationToken);
        }
    }

    public async Task<WorkflowProcessResult> PushReleasedEvent(WaitEventInfo releasedEventInfo, CancellationToken cancellationToken)
    {
        var executionContext = this.CreateExecutionContext(releasedEventInfo.TargetState, cancellationToken, releasedEventInfo);

        return await this.ProcessCurrentState(executionContext);
    }
}