using Anch.Core;
using Anch.Workflow.Domain;
using Anch.Workflow.Domain.Definition;
using Anch.Workflow.Domain.Runtime;
using Anch.Workflow.Execution;
using Anch.Workflow.Serialization;
using Anch.Workflow.StateFactory;

namespace Anch.Workflow.Engine;

public class WorkflowMachine(
    IServiceProvider serviceProvider,
    IWorkflowExecutor workflowExecutor,
    ICodeStateProcessorFactory codeStateProcessorFactory,
    IWorkflowRepository storage,
    WorkflowInstance workflowInstance,
    IWorkflowEventListener? eventListener = null)
    : IWorkflowMachine
{
    public WorkflowInstance WorkflowInstance { get; } = workflowInstance;

    public void SetStartState()
    {
        this.SetCurrentState(this.WorkflowInstance.Definition.StartState);
    }

    public async ValueTask Save(CancellationToken cancellationToken)
    {
        await storage.SaveWorkflowInstance(this.WorkflowInstance, cancellationToken);
    }

    public async ValueTask<WorkflowProcessResult> ProcessWorkflow(CancellationToken cancellationToken)
    {
        return await this.ProcessCurrentState(this.CreateExecutionContext(cancellationToken));
    }

    public async ValueTask<WorkflowProcessResult> ProcessWorkflow(ExecutionResult executionResult, CancellationToken cancellationToken) =>
        executionResult.WorkflowProcessResult + await this.ProcessExecutionResult(this.WorkflowInstance.CurrentState, executionResult, cancellationToken);

    public async ValueTask<WorkflowProcessResult> Terminate(CancellationToken cancellationToken)
    {
        if (this.WorkflowInstance.Status.Role != WorkflowStatusRole.Finished)
        {
            return await this.GetLeaveResult(
                       codeStateProcessorFactory.Create(this.WorkflowInstance.CurrentState),
                       this.CreateExecutionContext(cancellationToken), true)
                   + this.SwitchState(this.WorkflowInstance.Definition.TerminateState);
        }
        else
        {
            return WorkflowProcessResult.Empty;
        }
    }

    protected virtual IExecutionContext CreateExecutionContext(CancellationToken cancellationToken, WaitEventInfo? callbackEventInfo = null)
    {
        return this.CreateExecutionContext(this.WorkflowInstance.CurrentState, cancellationToken, callbackEventInfo);
    }

    protected virtual IExecutionContext CreateExecutionContext(StateInstance stateInstance, CancellationToken cancellationToken,
        WaitEventInfo? callbackEventInfo)
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

    private WorkflowProcessResult SwitchState(IStateDefinition newStateDefinition)
    {
        this.SetCurrentState(newStateDefinition);

        return new WorkflowProcessResult([this.WorkflowInstance], [new UnprocessedCurrentStateResult(this.WorkflowInstance)]);
    }

    private async ValueTask<WorkflowProcessResult> GetLeaveResult(ICodeStateProcessor codeStateProcessor, IExecutionContext executionContext, bool force)
    {
        var currentState = this.WorkflowInstance.CurrentState;

        if (!currentState.OutputProcessed)
        {
            currentState.OutputProcessed = true;

            if (!force)
            {
                await codeStateProcessor.BindOutput(executionContext.CancellationToken);
            }

            var leaveResult = await codeStateProcessor.CodeState.LeavePolicy.Leave(serviceProvider, executionContext);

            currentState.ReleaseWaitEvents();

            return leaveResult;
        }
        else
        {
            return WorkflowProcessResult.Empty;
        }
    }

    private async ValueTask<WorkflowProcessResult> ProcessCurrentState(IExecutionContext executionContext)
    {
        if (!executionContext.StateInstance.IsActual)
        {
            return WorkflowProcessResult.Empty;
        }

        executionContext.WorkflowInstance.Status = WorkflowStatus.Runnable;

        await this.Save(executionContext.CancellationToken);

        var currentState = executionContext.StateInstance;
        var codeStateProcessor = codeStateProcessorFactory.Create(currentState);

        if (!currentState.InputProcessed)
        {
            currentState.InputProcessed = true;

            await codeStateProcessor.BindInput(executionContext.CancellationToken);

            codeStateProcessor.SetStatus();
        }

        var runExecutionResult = await codeStateProcessor.CodeState.Run(executionContext);

        var modifyResult = new WorkflowProcessResult([this.WorkflowInstance], []);

        var runResult = new WorkflowProcessResult([], [new UnprocessedStateResult(currentState, runExecutionResult)]);

        if (runExecutionResult.LeaveState)
        {
            var leaveResult = await this.GetLeaveResult(codeStateProcessor, executionContext, false);

            return modifyResult + leaveResult + runResult;
        }
        else
        {
            return modifyResult + runResult;
        }
    }

    protected virtual async ValueTask<WorkflowProcessResult> ProcessExecutionResult(StateInstance stateInstance, ExecutionResult executionResult,
        CancellationToken cancellationToken)
    {
        switch (executionResult)
        {
            case WaitAnyEvent:
                stateInstance.Workflow.Status = WorkflowStatus.WaitEvent;
                return WorkflowProcessResult.Empty;

            case WaitEventResult waitEventResult:
                stateInstance.Workflow.Status = WorkflowStatus.WaitEvent;
                stateInstance.RegisterWaitEvent(waitEventResult.ToEventInfo(stateInstance));
                return WorkflowProcessResult.Empty;

            case PushEventResult pushEventResult:
                return await this.ProcessExecutionResult(stateInstance, pushEventResult, cancellationToken);

            case Done:
                return await this.ProcessExecutionResult(stateInstance, new PushEventResult(EventHeader.StateDone, stateInstance), cancellationToken);

            case MultiExecutionResult multiExecutionResult:
                return new WorkflowProcessResult([],
                    [.. multiExecutionResult.ExecutionResults.Select(er => new UnprocessedStateResult(stateInstance, er))]);

            default:
                throw new ArgumentOutOfRangeException(nameof(executionResult));
        }
    }

    private async ValueTask<WorkflowProcessResult> ProcessExecutionResult(StateInstance stateInstance, PushEventResult pushEventResult,
        CancellationToken cancellationToken)
    {
        return await this.ProcessExecutionResult(stateInstance, pushEventResult.ToEventInfo(stateInstance.Workflow), cancellationToken);
    }

    private async ValueTask<WorkflowProcessResult> ProcessExecutionResult(StateInstance stateInstance, PushEventInfo pushEventInfo,
        CancellationToken cancellationToken)
    {
        if (pushEventInfo.TargetState == null && pushEventInfo.Header.IsGlobal)
        {
            if (pushEventInfo.Header == EventHeader.WorkflowFinished)
            {
                if (stateInstance.Workflow.Status.Role != WorkflowStatusRole.Finished)
                {
                    stateInstance.Workflow.Status = WorkflowStatus.Finished;
                }
            }
            else if (pushEventInfo.Header == EventHeader.WorkflowTerminated)
            {
                stateInstance.Workflow.Status = WorkflowStatus.Terminated;
            }

            return await workflowExecutor.PushEvent(pushEventInfo, cancellationToken);
        }
        else if (pushEventInfo.TargetState.Maybe(s => s.Workflow != stateInstance.Workflow))
        {
            return await workflowExecutor.PushEvent(pushEventInfo, cancellationToken);
        }
        else
        {
            var transition = stateInstance.Definition.Transitions.Single(tr => tr.Event.Header == pushEventInfo.Header);

            return this.SwitchState(transition.To);
        }
    }

    public async ValueTask<WorkflowProcessResult> PushReleasedEvent(WaitEventInfo releasedEventInfo, CancellationToken cancellationToken)
    {
        var executionContext = this.CreateExecutionContext(releasedEventInfo.TargetState, cancellationToken, releasedEventInfo);

        return await this.ProcessCurrentState(executionContext);
    }
}