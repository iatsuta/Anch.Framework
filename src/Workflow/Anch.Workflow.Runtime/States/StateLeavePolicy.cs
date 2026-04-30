using Anch.Workflow.Engine;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.Workflow.States;

public class StateLeavePolicy(Func<IServiceProvider, IExecutionContext, Task<WorkflowProcessResult>> action)
{
    public async Task<WorkflowProcessResult> Leave(IServiceProvider serviceProvider, IExecutionContext executionContext)
    {
        return await action(serviceProvider, executionContext);
    }


    public static StateLeavePolicy Forget { get; } = new(async (_, __) => WorkflowProcessResult.Empty);

    public static StateLeavePolicy TerminateChild { get; } = new(async (serviceProvider, executionContext) =>
    {
        var host = serviceProvider.GetRequiredService<IWorkflowHost>();

        var notFinishedInstances = executionContext.StateInstance.Child.Where(wi => wi.Status.Role != WorkflowStatusRole.Finished).ToList();

        var result = new List<WorkflowProcessResult>();
        
        foreach (var wi in notFinishedInstances)
        {
            result.Add(await host.CreateMachine(wi).Terminate(executionContext.CancellationToken));
        }

        return result.Aggregate();
    });
}