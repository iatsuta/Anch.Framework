using System.Collections.Frozen;

using Anch.Core;
using Anch.Workflow.Domain.Definition;

namespace Anch.Workflow.Persistence;

public class WorkflowSource(IEnumerable<IWorkflow> rootWorkflows) : IWorkflowSource
{
    public FrozenDictionary<WorkflowDefinitionIdentity, IWorkflowDefinition> Workflows =>
        field ??= rootWorkflows.Select(wf => wf.Definition)
            .GetAllElements(wfRef => wfRef.States.SelectMany(state => state.SubWorkflows))
            .ToFrozenDictionary(wfRef => wfRef.Identity);
}