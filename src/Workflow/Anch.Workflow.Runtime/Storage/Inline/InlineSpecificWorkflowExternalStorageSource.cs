using Microsoft.Extensions.DependencyInjection;

namespace Anch.Workflow.Storage.Inline;

public class InlineSpecificWorkflowExternalStorageSource(IWorkflowSource workflowSource, IServiceProvider serviceProvider)
    : SpecificWorkflowExternalStorageSource(workflowSource)
{
    //
    protected override ISpecificWorkflowExternalStorage CreateSpecificWorkflowExternalStorage(IWorkflow wfRef)
    {
        var storageType = typeof(InlineSpecificWorkflowExternalStorage<>).MakeGenericType(wfRef.Definition.SourceType);

        return (ISpecificWorkflowExternalStorage)ActivatorUtilities.CreateInstance(serviceProvider, storageType, wfRef);
    }
}
