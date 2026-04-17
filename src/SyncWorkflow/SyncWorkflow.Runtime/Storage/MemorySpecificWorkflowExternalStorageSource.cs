namespace SyncWorkflow.Storage;

public class MemorySpecificWorkflowExternalStorageSource(
    IWorkflowSource workflowSource,
    IServiceProvider serviceProvider)
    : SpecificWorkflowExternalStorageSource<MemorySpecificWorkflowExternalStorage>(workflowSource, serviceProvider);