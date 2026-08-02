//using Anch.Core;
//using Anch.Workflow.Domain;
//using Anch.Workflow.Domain.Runtime;
//using Anch.Workflow.Execution;
//using Anch.Workflow.Serialization;
//using Anch.Workflow.StateFactory;
//using Anch.Workflow.States;

//namespace Anch.Workflow.Engine;

//public class WorkflowStateMachine<TState, TSource>(
//    IServiceProvider serviceProvider,
//    IWorkflowHost host,
//    IStateFactoryCache stateFactoryCache,
//    ISpecificWorkflowRepository storage,
//    StateInstance stateInstance)
//    where TState : IState
//    where TSource : notnull
//{
//    private WorkflowInstance WorkflowInstance => stateInstance.Workflow;

//    private TSource Source => field ??= (TSource)this.WorkflowInstance.Source;

//    private IStateFactory StateFactory => field ??= stateFactoryCache.GetStateFactory(stateInstance.Definition);

//    private TState CodeState => field ??= (TState)this.StateFactory.CreateState(serviceProvider, this.Source);

//    public async ValueTask Save(CancellationToken ct) =>

//        await storage.SaveWorkflowInstance(this.WorkflowInstance, ct);



//    protected virtual IExecutionContext CreateExecutionContext(CancellationToken ct, WaitEventInfo? callbackEventInfo = null) =>

//        new ExecutionContext
//        {
//            StateInstance = stateInstance,
//            CancellationToken = ct,
//            CallbackEventInfo = callbackEventInfo
//        };

//    public async ValueTask<WorkflowProcessResult> ProcessCurrentState(CancellationToken ct)
//    {
//        if (!stateInstance.IsActual)
//        {
//            return WorkflowProcessResult.Empty;
//        }

//        this.WorkflowInstance.SetStatus(WorkflowStatus.Runnable);

//        await this.Save(ct);

//        if (!stateInstance.InputProcessed)
//        {
//            stateInstance.InputProcessed = true;

//            await this.StateFactory.BindInput(this.CodeState, serviceProvider, this.Source, ct);
//        }

//        var executionContext = this.CreateExecutionContext(ct);

//        var executionResult = await this.CodeState.Run(executionContext);

//        var modifyResult = new WorkflowProcessResult([this.WorkflowInstance], []);

//        var runResult = new WorkflowProcessResult([], [new UnprocessedStateResult(stateInstance, executionResult)]);

//        if (executionResult.LeaveState && !stateInstance.OutputProcessed)
//        {
//            stateInstance.OutputProcessed = true;

//            await this.StateFactory.BindOutput(this.CodeState, serviceProvider, this.Source, ct);

//            var leaveResult = await this.CodeState.LeavePolicy.Leave(serviceProvider, executionContext);

//            return modifyResult + leaveResult + runResult;
//        }
//        else
//        {
//            return modifyResult + runResult;
//        }
//    }

//    public virtual async ValueTask<WorkflowProcessResult> ProcessExecutionResult(IExecutionResult executionResult, CancellationToken ct)
//    {
//        switch (executionResult)
//        {
//            case WorkflowProcessExecutionResult workflowProcessExecutionResult:
//            {
//                if (workflowProcessExecutionResult.LeaveState)
//                {
//                    return workflowProcessExecutionResult.WorkflowProcessResult +

//                           await this.ProcessExecutionResult(stateInstance, new PushEventResult(EventHeader.StateDone, stateInstance), ct);
//                }
//                else
//                {
//                    return workflowProcessExecutionResult.WorkflowProcessResult;
//                }
//            }

//            case Wait:
//                this.WorkflowInstance.SetStatus(WorkflowStatus.WaitEvent);
//                return WorkflowProcessResult.Empty;

//            case WaitEventResult waitEventResult:
//                this.WorkflowInstance.SetStatus(WorkflowStatus.WaitEvent);
//                stateInstance.RegisterWaitEvent(waitEventResult.ToEventInfo(stateInstance));
//                return WorkflowProcessResult.Empty;

//            case Done:
//                return await this.ProcessExecutionResult(new PushEventResult(EventHeader.StateDone, stateInstance), ct);

//            case PushEventResult pushEventResult:
//                return await this.ProcessExecutionResult(pushEventResult, ct);

//            case MultiExecutionResult multiExecutionResult:
//                return new WorkflowProcessResult([],
//                    [.. multiExecutionResult.ExecutionResults.Select(er => new UnprocessedStateResult(stateInstance, er))]);

//            default:
//                throw new ArgumentOutOfRangeException(nameof(executionResult));
//        }
//    }

//    private async ValueTask<WorkflowProcessResult> ProcessExecutionResult(PushEventResult pushEventResult, CancellationToken ct)
//    {
//        return await this.ProcessExecutionResult(pushEventResult.ToEventInfo(this.WorkflowInstance), ct);
//    }

//    private async ValueTask<WorkflowProcessResult> ProcessExecutionResult(PushEventInfo pushEventInfo, CancellationToken ct)
//    {
//        if (pushEventInfo.TargetState == null && pushEventInfo.Header.IsGlobal)
//        {
//            if (pushEventInfo.Header == EventHeader.WorkflowFinished)
//            {
//                if (this.WorkflowInstance.Status.Role != WorkflowStatusRole.Finished)
//                {
//                    this.WorkflowInstance.SetStatus(WorkflowStatus.Finished);
//                }
//            }
//            else if (pushEventInfo.Header == EventHeader.WorkflowTerminated)
//            {
//                this.WorkflowInstance.SetStatus(WorkflowStatus.Terminated);
//            }

//            return await host.CreateExecutor(WorkflowExecutionPolicy.SingleStep).PushEvent(pushEventInfo, ct);
//        }
//        else if (pushEventInfo.TargetState.Maybe(s => s.Workflow != this.WorkflowInstance))
//        {
//            return await host.CreateExecutor(WorkflowExecutionPolicy.SingleStep).PushEvent(pushEventInfo, ct);
//        }
//        else
//        {
//            var transition = stateInstance.Definition.Transitions.Single(tr => tr.Event.Header == pushEventInfo.Header);

//            return await this.SwitchState(transition.To, ct);
//        }
//    }

//    public async ValueTask<WorkflowProcessResult> PushReleasedEvent(WaitEventInfo releasedEventInfo, CancellationToken ct)
//    {
//        var executionContext = this.CreateExecutionContext(releasedEventInfo.TargetState, ct, releasedEventInfo);

//        return await this.ProcessCurrentState(executionContext);
//    }
//}