using Anch.Workflow.Domain;
using Anch.Workflow.Domain.Definition;
using Anch.Workflow.Domain.Runtime;
using Anch.Workflow.Execution;
using Anch.Workflow.Serialization;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.Workflow.Engine;

public class WorkflowExecutor(
    IWorkflowMachineFactory workflowMachineFactory,
    IServiceProvider serviceProvider,
    [FromKeyedServices(IWorkflowRepository.RootKey)]
    IWorkflowRepository workflowRootRepository,
    WorkflowExecutionPolicy executionPolicy)
    : IWorkflowExecutor
{
    public ValueTask<WorkflowProcessResult> Start<TSource, TWorkflow>(TSource source, CancellationToken ct)
        where TSource : notnull
        where TWorkflow : IWorkflow<TSource> =>

        this.Start(source, serviceProvider.GetRequiredService<TWorkflow>(), ct);

    public ValueTask<WorkflowProcessResult> Start<TSource>(TSource source, IWorkflow<TSource> workflow, CancellationToken ct)
        where TSource : notnull =>
        this.Start(source, workflow.Definition, ct);

    public async ValueTask<WorkflowProcessResult> Start<TSource>(TSource source, IWorkflowDefinition<TSource> workflowDefinition, CancellationToken ct)
        where TSource : notnull
    {
        var machine = workflowMachineFactory.Create(source, workflowDefinition);

        var preResult = await machine.ProcessWorkflow(ct);

        return await this.ProcessUnprocessed(preResult, true, ct);
    }

    public async ValueTask<WorkflowProcessResult> PushEvent(PushEventInfo pushEventInfo, CancellationToken ct)
    {
        var waitEvents = await workflowRootRepository.GetWaitEvents(pushEventInfo).ToListAsync(ct);

        foreach (var waitEventInfo in waitEvents)
        {
            waitEventInfo.Release();
        }

        var preResult = await waitEvents
            .AggregateAsync((waitEventInfo, lct) => workflowMachineFactory
                .Create(waitEventInfo.TargetState.Workflow)
                .PushReleasedEvent(waitEventInfo with { Data = pushEventInfo.Data }, lct), ct);

        return await this.ProcessUnprocessed(preResult, true, ct);
    }

    public ValueTask<WorkflowProcessResult> ProcessUnprocessed(WorkflowProcessResult workflowProcessResult, CancellationToken ct) =>

        this.ProcessUnprocessed(workflowProcessResult, false, ct);

    public async ValueTask<WorkflowProcessResult> Terminate(WorkflowInstance workflowInstance, CancellationToken ct)
    {
        var terminateResult = await workflowMachineFactory.Create(workflowInstance).Terminate(ct);

        return await this.ProcessUnprocessed(terminateResult, true, ct);
    }

    private async ValueTask<WorkflowProcessResult> ProcessUnprocessed(
        WorkflowProcessResult preWorkflowProcessResult,
        bool preFirstStepProcessed,
        CancellationToken ct)
    {
        var workflowProcessResult = preWorkflowProcessResult;
        var firstStepProcessed = preFirstStepProcessed;

        while (!workflowProcessResult.Unprocessed.IsEmpty && !this.StopProcess(firstStepProcessed))
        {
            var modified = workflowProcessResult.Modified;
            var tailUnprocessed = workflowProcessResult.Unprocessed.Pop(out var current);

            var stepResult = await this.ProcessStep(current, ct);

            firstStepProcessed = true;

            workflowProcessResult = new WorkflowProcessResult(modified, []) + stepResult + new WorkflowProcessResult([], tailUnprocessed);
        }

        return workflowProcessResult;
    }

    private async ValueTask<WorkflowProcessResult> ProcessStep(UnprocessedStateResultBase unprocessedStateResultBase, CancellationToken ct)
    {
        switch (unprocessedStateResultBase)
        {
            case UnprocessedStateResult unprocessedStateResult:
                {
                    var machine = workflowMachineFactory.Create(unprocessedStateResult.StateInstance.Workflow);

                    return await machine.ProcessWorkflow(unprocessedStateResult.ExecutionResult, ct);
                }

            case UnprocessedCurrentStateResult unprocessedCurrentStateResult:
                {
                    var machine = workflowMachineFactory.Create(unprocessedCurrentStateResult.WorkflowInstance);

                    return await machine.ProcessWorkflow(ct);
                }

            default:
                throw new ArgumentOutOfRangeException(nameof(unprocessedStateResultBase), unprocessedStateResultBase, null);
        }
    }

    private bool StopProcess(bool firstStepProcessed) => firstStepProcessed && executionPolicy == WorkflowExecutionPolicy.SingleStep;
}