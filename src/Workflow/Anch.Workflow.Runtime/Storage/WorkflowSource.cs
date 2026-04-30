using Anch.Core;
using Anch.Workflow.Definition;

namespace Anch.Workflow.Storage;

public class WorkflowSource : IWorkflowSource
{
    private readonly Lazy<Dictionary<WorkflowDefinitionIdentity, IWorkflow>> lazyStorageDict;

    public WorkflowSource(IEnumerable<IWorkflow> rootWorkflows)
    {
        this.lazyStorageDict = LazyHelper.Create(() =>
        {
            return rootWorkflows
                .GetAllElements(wfRef => wfRef.Definition.States.SelectMany(state => state.SubWorkflow))
                .ToDictionary(wfRef => wfRef.Definition.Identity);
        });
    }

    public IReadOnlyDictionary<WorkflowDefinitionIdentity, IWorkflow> GetWorkflows()
    {
        return this.lazyStorageDict.Value;
    }
}