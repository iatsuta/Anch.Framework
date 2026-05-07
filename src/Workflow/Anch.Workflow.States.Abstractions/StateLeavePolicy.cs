using Anch.Workflow.Domain.Runtime;
using Anch.Workflow.Engine;
using Anch.Workflow.Execution;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.Workflow.States;

public class StateLeavePolicy(Func<IServiceProvider, IExecutionContext, ValueTask<WorkflowProcessResult>> action)
{
    public async ValueTask<WorkflowProcessResult> Leave(IServiceProvider serviceProvider, IExecutionContext executionContext)
    {
        return await action(serviceProvider, executionContext);
    }


    public static StateLeavePolicy Forget { get; } = new(async (_, __) => WorkflowProcessResult.Empty);

    public static StateLeavePolicy TerminateChild { get; } = new(async (serviceProvider, executionContext) =>
    {
        var workflowMachineFactory = serviceProvider.GetRequiredService<IWorkflowMachineFactory>();

        var notFinishedInstances = executionContext.StateInstance.Children.Where(wi => wi.Status.Role != WorkflowStatusRole.Finished).ToList();

        var result = new List<WorkflowProcessResult>();

        foreach (var wi in notFinishedInstances)
        {
            result.Add(await workflowMachineFactory.Create(wi).Terminate(executionContext.CancellationToken));
        }

        return result.Aggregate();
    });
}