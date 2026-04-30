using FluentNHibernate.Mapping;

using SyncWorkflow.Tests.StartWorkflowsWithTaskApprove;

namespace SyncWorkflow.IntegrationTests.NHib;

public class StartWorkflowsWithTaskApproveWorkflowObjectMapping : ClassMap<StartWorkflowsWithTaskApproveWorkflowObject>
{
    public StartWorkflowsWithTaskApproveWorkflowObjectMapping()
    {
        this.Schema("app");
        this.Id(x => x.Id).GeneratedBy.GuidComb();
        this.Map(x => x.Status).Access.CamelCaseField();
        this.HasMany(x => x.Items).Cascade.All();
    }
}