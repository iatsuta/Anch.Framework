using CommonFramework;
using Microsoft.Extensions.DependencyInjection;
using SyncWorkflow.Domain.Definition;

namespace SyncWorkflow.Storage;

public abstract class SpecificWorkflowExternalStorageSource : ISpecificWorkflowExternalStorageSource
{
    private readonly Lazy<Dictionary<WorkflowDefinitionIdentity, ISpecificWorkflowExternalStorage>> lazyStorageDict;

    protected SpecificWorkflowExternalStorageSource(IWorkflowSource workflowSource)
    {
        this.lazyStorageDict = LazyHelper.Create(() =>
            workflowSource.GetWorkflows().ChangeValue(this.CreateSpecificWorkflowExternalStorage));
    }

    protected abstract ISpecificWorkflowExternalStorage CreateSpecificWorkflowExternalStorage(IWorkflow wfRef);

    public IReadOnlyDictionary<WorkflowDefinitionIdentity, ISpecificWorkflowExternalStorage> GetSpecificStorageDict()
    {
        return this.lazyStorageDict.Value;
    }
}

public class SpecificWorkflowExternalStorageSource<TSpecificWorkflowExternalStorage>(IWorkflowSource workflowSource,
        IServiceProvider serviceProvider)
    : SpecificWorkflowExternalStorageSource(workflowSource)
    where TSpecificWorkflowExternalStorage : ISpecificWorkflowExternalStorage
{
    protected override ISpecificWorkflowExternalStorage CreateSpecificWorkflowExternalStorage(IWorkflow wfRef)
    {
        return ActivatorUtilities.CreateInstance<TSpecificWorkflowExternalStorage>(serviceProvider, wfRef);
    }
}