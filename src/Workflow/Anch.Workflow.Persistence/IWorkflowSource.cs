using System.Collections.Frozen;

using Anch.Workflow.Domain.Definition;

namespace Anch.Workflow.Persistence;

public interface IWorkflowSource
{
    FrozenDictionary<WorkflowDefinitionIdentity, IWorkflowDefinition> Workflows { get; }
}