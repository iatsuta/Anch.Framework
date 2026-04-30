using FluentNHibernate.Mapping;

using SyncWorkflow.Tests.StartWorkflowsWithTaskApprove;

namespace SyncWorkflow.IntegrationTests.NHib;

public class StartWorkflowsWithTaskApproveItemWorkflowObjectMapping : ClassMap<StartWorkflowsWithTaskApproveItemWorkflowObject>
{
    public StartWorkflowsWithTaskApproveItemWorkflowObjectMapping()
    {
        this.Schema("app");
        this.Id(x => x.Id).GeneratedBy.GuidComb();
        this.Map(x => x.Status).Access.CamelCaseField();
        this.Map(x => x.Name).Access.CamelCaseField();
    }
}