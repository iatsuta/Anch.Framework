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
    public ValueTask<WorkflowProcessResult> Start<TSource, TWorkflow>(TSource source, CancellationToken cancellationToken)
        where TSource : notnull
        where TWorkflow : IWorkflow<TSource> =>

        this.Start(source, serviceProvider.GetRequiredService<TWorkflow>(), cancellationToken);

    public ValueTask<WorkflowProcessResult> Start<TSource>(TSource source, IWorkflow<TSource> workflow, CancellationToken cancellationToken)
        where TSource : notnull =>
        this.Start(source, workflow.Definition, cancellationToken);

    public async ValueTask<WorkflowProcessResult> Start<TSource>(TSource source, IWorkflowDefinition<TSource> workflowDefinition, CancellationToken cancellationToken)
        where TSource : notnull
    {
        var machine = workflowMachineFactory.Create(source, workflowDefinition);

        var preResult = await machine.ProcessWorkflow(cancellationToken);

        return await this.ProcessUnprocessed(preResult, true, cancellationToken);
    }

    public async ValueTask<WorkflowProcessResult> PushEvent(PushEventInfo pushEventInfo, CancellationToken cancellationToken)
    {
        var waitEvents = await workflowRootRepository.GetWaitEvents(pushEventInfo).ToListAsync(cancellationToken);

        foreach (var waitEventInfo in waitEvents)
        {
            waitEventInfo.Release();
        }

        var preResult = await waitEvents
            .AggregateAsync((waitEventInfo, ct) => workflowMachineFactory
                .Create(waitEventInfo.TargetState.Workflow)
                .PushReleasedEvent(waitEventInfo with { Data = pushEventInfo.Data }, ct), cancellationToken);

        return await this.ProcessUnprocessed(preResult, true, cancellationToken);
    }

    public ValueTask<WorkflowProcessResult> ProcessUnprocessed(WorkflowProcessResult workflowProcessResult, CancellationToken cancellationToken) =>

        this.ProcessUnprocessed(workflowProcessResult, false, cancellationToken);

    public async ValueTask<WorkflowProcessResult> Terminate(WorkflowInstance workflowInstance, CancellationToken cancellationToken)
    {
        var terminateResult = await workflowMachineFactory.Create(workflowInstance).Terminate(cancellationToken);

        return await this.ProcessUnprocessed(terminateResult, true, cancellationToken);
    }

    private async ValueTask<WorkflowProcessResult> ProcessUnprocessed(
        WorkflowProcessResult preWorkflowProcessResult,
        bool preFirstStepProcessed,
        CancellationToken cancellationToken)
    {
        var workflowProcessResult = preWorkflowProcessResult;
        var firstStepProcessed = preFirstStepProcessed;

        while (!workflowProcessResult.Unprocessed.IsEmpty && !this.StopProcess(firstStepProcessed))
        {
            var modified = workflowProcessResult.Modified;
            var tailUnprocessed = workflowProcessResult.Unprocessed.Pop(out var current);

            var stepResult = await this.ProcessStep(current, cancellationToken);

            firstStepProcessed = true;

            workflowProcessResult = new WorkflowProcessResult(modified, []) + stepResult + new WorkflowProcessResult([], tailUnprocessed);
        }

        return workflowProcessResult;
    }

    private async ValueTask<WorkflowProcessResult> ProcessStep(UnprocessedStateResultBase unprocessedStateResultBase, CancellationToken cancellationToken)
    {
        switch (unprocessedStateResultBase)
        {
            case UnprocessedStateResult unprocessedStateResult:
            {
                var machine = workflowMachineFactory.Create(unprocessedStateResult.StateInstance.Workflow);

                return await machine.ProcessWorkflow(unprocessedStateResult.ExecutionResult, cancellationToken);
            }

            case UnprocessedCurrentStateResult unprocessedCurrentStateResult:
            {
                var machine = workflowMachineFactory.Create(unprocessedCurrentStateResult.WorkflowInstance);

                return await machine.ProcessWorkflow(cancellationToken);
            }

            default:
                throw new ArgumentOutOfRangeException(nameof(unprocessedStateResultBase), unprocessedStateResultBase, null);
        }
    }

    private bool StopProcess(bool firstStepProcessed) => firstStepProcessed && executionPolicy == WorkflowExecutionPolicy.SingleStep;
}