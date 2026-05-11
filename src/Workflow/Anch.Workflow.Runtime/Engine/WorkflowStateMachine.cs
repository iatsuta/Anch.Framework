//using Anch.Core;
//using Anch.Workflow.Domain;
//using Anch.Workflow.Domain.Runtime;
//using Anch.Workflow.Execution;
//using Anch.Workflow.Persistence;
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
//    where TSource : class
//{
//    private WorkflowInstance WorkflowInstance => stateInstance.Workflow;

//    private TSource Source => field ??= (TSource)this.WorkflowInstance.Source;

//    private IStateFactory StateFactory => field ??= stateFactoryCache.GetStateFactory(stateInstance.Definition);

//    private TState CodeState => field ??= (TState)this.StateFactory.CreateState(serviceProvider, this.Source);

//    public async ValueTask Save(CancellationToken cancellationToken) =>

//        await storage.SaveWorkflowInstance(this.WorkflowInstance, cancellationToken);



//    protected virtual IExecutionContext CreateExecutionContext(CancellationToken cancellationToken, WaitEventInfo? callbackEventInfo = null) =>

//        new ExecutionContext
//        {
//            StateInstance = stateInstance,
//            CancellationToken = cancellationToken,
//            CallbackEventInfo = callbackEventInfo
//        };

//    public async ValueTask<WorkflowProcessResult> ProcessCurrentState(CancellationToken cancellationToken)
//    {
//        if (!stateInstance.IsActual)
//        {
//            return WorkflowProcessResult.Empty;
//        }

//        this.WorkflowInstance.SetStatus(WorkflowStatus.Runnable);

//        await this.Save(cancellationToken);

//        if (!stateInstance.InputProcessed)
//        {
//            stateInstance.InputProcessed = true;

//            await this.StateFactory.BindInput(this.CodeState, serviceProvider, this.Source, cancellationToken);
//        }

//        var executionContext = this.CreateExecutionContext(cancellationToken);

//        var executionResult = await this.CodeState.Run(executionContext);

//        var modifyResult = new WorkflowProcessResult([this.WorkflowInstance], []);

//        var runResult = new WorkflowProcessResult([], [new UnprocessedStateResult(stateInstance, executionResult)]);

//        if (executionResult.LeaveState && !stateInstance.OutputProcessed)
//        {
//            stateInstance.OutputProcessed = true;

//            await this.StateFactory.BindOutput(this.CodeState, serviceProvider, this.Source, cancellationToken);

//            var leaveResult = await this.CodeState.LeavePolicy.Leave(serviceProvider, executionContext);

//            return modifyResult + leaveResult + runResult;
//        }
//        else
//        {
//            return modifyResult + runResult;
//        }
//    }

//    public virtual async ValueTask<WorkflowProcessResult> ProcessExecutionResult(IExecutionResult executionResult, CancellationToken cancellationToken)
//    {
//        switch (executionResult)
//        {
//            case WorkflowProcessExecutionResult workflowProcessExecutionResult:
//            {
//                if (workflowProcessExecutionResult.LeaveState)
//                {
//                    return workflowProcessExecutionResult.WorkflowProcessResult +

//                           await this.ProcessExecutionResult(stateInstance, new PushEventResult(EventHeader.StateDone, stateInstance), cancellationToken);
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
//                return await this.ProcessExecutionResult(new PushEventResult(EventHeader.StateDone, stateInstance), cancellationToken);

//            case PushEventResult pushEventResult:
//                return await this.ProcessExecutionResult(pushEventResult, cancellationToken);

//            case MultiExecutionResult multiExecutionResult:
//                return new WorkflowProcessResult([],
//                    [.. multiExecutionResult.ExecutionResults.Select(er => new UnprocessedStateResult(stateInstance, er))]);

//            default:
//                throw new ArgumentOutOfRangeException(nameof(executionResult));
//        }
//    }

//    private async ValueTask<WorkflowProcessResult> ProcessExecutionResult(PushEventResult pushEventResult, CancellationToken cancellationToken)
//    {
//        return await this.ProcessExecutionResult(pushEventResult.ToEventInfo(this.WorkflowInstance), cancellationToken);
//    }

//    private async ValueTask<WorkflowProcessResult> ProcessExecutionResult(PushEventInfo pushEventInfo, CancellationToken cancellationToken)
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

//            return await host.CreateExecutor(WorkflowExecutionPolicy.SingleStep).PushEvent(pushEventInfo, cancellationToken);
//        }
//        else if (pushEventInfo.TargetState.Maybe(s => s.Workflow != this.WorkflowInstance))
//        {
//            return await host.CreateExecutor(WorkflowExecutionPolicy.SingleStep).PushEvent(pushEventInfo, cancellationToken);
//        }
//        else
//        {
//            var transition = stateInstance.Definition.Transitions.Single(tr => tr.Event.Header == pushEventInfo.Header);

//            return await this.SwitchState(transition.To, cancellationToken);
//        }
//    }

//    public async ValueTask<WorkflowProcessResult> PushReleasedEvent(WaitEventInfo releasedEventInfo, CancellationToken cancellationToken)
//    {
//        var executionContext = this.CreateExecutionContext(releasedEventInfo.TargetState, cancellationToken, releasedEventInfo);

//        return await this.ProcessCurrentState(executionContext);
//    }
//}