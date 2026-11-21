using Anch.Workflow.Domain.Definition;

namespace Anch.Workflow.Building.Default.DomainDefinition;

public interface IWorkflowDefinitionBuilder : IWorkflowDefinition
{
    void ReplaceAutoNames((IStateDefinitionBuilder State, int? Index)? ownerInfo = null);
}