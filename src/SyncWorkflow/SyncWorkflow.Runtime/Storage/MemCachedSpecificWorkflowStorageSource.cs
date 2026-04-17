namespace SyncWorkflow.Storage;

public class MemCachedSpecificWorkflowStorageSource(
    IWorkflowSource workflowSource,
    ISpecificWorkflowExternalStorageSource externalStorageSource)
    : SpecificWorkflowStorageSource(workflowSource)
{
    protected override ISpecificWorkflowStorage CreateSpecificWorkflowStorage(IWorkflow wfRef)
    {
        var externalStorage = externalStorageSource.GetSpecificStorageDict()[wfRef.Definition.Identity];

        return new MemCachedSpecificWorkflowStorage(externalStorage, wfRef);
    }
}