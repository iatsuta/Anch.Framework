using CommonFramework;
using SyncWorkflow.Domain.Definition;

namespace SyncWorkflow.Storage;

public abstract class SpecificWorkflowStorageSource : ISpecificWorkflowStorageSource
{
    private readonly Lazy<Dictionary<WorkflowDefinitionIdentity, ISpecificWorkflowStorage>> lazyStorageDict;

    protected SpecificWorkflowStorageSource(IWorkflowSource workflowSource)
    {
        this.lazyStorageDict = LazyHelper.Create(() =>
            workflowSource.GetWorkflows().ChangeValue(this.CreateSpecificWorkflowStorage));
    }

    protected abstract ISpecificWorkflowStorage CreateSpecificWorkflowStorage(IWorkflow wfRef);

    public IReadOnlyDictionary<WorkflowDefinitionIdentity, ISpecificWorkflowStorage> GetSpecificStorageDict()
    {
        return this.lazyStorageDict.Value;
    }
}