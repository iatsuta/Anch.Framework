using Anch.Workflow.Domain;
using Anch.Workflow.Domain.Runtime;
using Anch.Workflow.Engine;
using Anch.Workflow.Execution;

namespace Anch.Workflow.States;

public class WaitEventState : IState
{
    public EventHeader Event { get; set; } = null!;

    public WorkflowInstance? SourceWorkflow { get; set; }

    public object? ExpectedData { get; set; }

    public object? ReceivedData { get; set; }

    public async ValueTask<ExecutionResult> Run(IExecutionContext executionContext)
    {
        if (executionContext.CallbackEventInfo is { } callbackEventInfo)
        {
            this.ReceivedData = callbackEventInfo.Data;

            return new Done();
        }
        else
        {
            return new WaitEventResult(this.Event, this.SourceWorkflow, this.ExpectedData);
        }
    }
}

//public class WaitEventState<TSource, TData> : IState
//{
//    public EventHeader Event { get; set; } = null!;

//    public WorkflowInstance? SourceWorkflow { get; set; }

//    public Func<TSource, TData, CancellationToken, ValueTask>? Callback { get; set; }

//    public async ValueTask<IExecutionResult> Run(IExecutionContext executionContext)
//    {
//        if (executionContext.IsCallbackEvent)
//        {
//            var data = (TData)executionContext.CallbackEventInfo!.Data!;

//            if (this.Callback != null)
//            {
//                await this.Callback((TSource)executionContext.Source, data, executionContext.CancellationToken);
//            }

//            return new Done();
//        }

//        return new WaitEventResult(this.Event, this.SourceWorkflow);
//    }
//}