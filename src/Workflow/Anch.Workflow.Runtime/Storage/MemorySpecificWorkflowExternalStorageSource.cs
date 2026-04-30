namespace Anch.Workflow.Storage;

public class MemorySpecificWorkflowExternalStorageSource(
    IWorkflowSource workflowSource,
    IServiceProvider serviceProvider)
    : SpecificWorkflowExternalStorageSource<MemorySpecificWorkflowExternalStorage>(workflowSource, serviceProvider);