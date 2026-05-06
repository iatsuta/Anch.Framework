using Anch.Workflow.DependencyInjection;
using Anch.Workflow.Domain.Runtime;
using Anch.Workflow.Execution;
using Anch.Workflow.Serialization;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.Workflow.Tests._Base;

public abstract class SingleScopeWorkflowTestBase<TSource, TWorkflow> : MultiScopeWorkflowTestBase
    where TWorkflow : class, IWorkflow<TSource>, new()
    where TSource : notnull
{
    private readonly Lazy<IServiceScope> lazyScope;

    protected SingleScopeWorkflowTestBase()
    {
        this.lazyScope = new Lazy<IServiceScope>(() => this.RootServiceProvider.CreateScope());
    }

    protected IWorkflowHost Host => this.ScopeServiceProvider.GetWorkflowHost();

    protected IWorkflowExecutor TillTheEndWorkflowExecutor => this.Host.CreateExecutor(WorkflowExecutionPolicy.TillTheEnd);

    protected IWorkflowRepository RootRepository => this.ScopeServiceProvider.GetRootRepository();

    protected IServiceProvider ScopeServiceProvider => this.lazyScope.Value.ServiceProvider;

    protected async ValueTask<WorkflowInstance> StartWorkflow(TSource source, CancellationToken cancellationToken)
    {
        var wfStartResult = await this.StartWorkflowNative(source, cancellationToken);

        return wfStartResult.Modified.First();
    }

    protected async ValueTask<WorkflowProcessResult> StartWorkflowNative(TSource source, CancellationToken cancellationToken) =>

        await this.TillTheEndWorkflowExecutor.Start<TSource, TWorkflow>(source, cancellationToken);

    protected override void SetupWorkflow(IWorkflowSetup workflowSetup) => workflowSetup.Add<TWorkflow>();
}