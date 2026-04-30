using FluentNHibernate.Mapping;

using SyncWorkflow.Tests._TaskState;

namespace SyncWorkflow.IntegrationTests.NHib;

public class TaskWorkflowObjectMapping : ClassMap<TaskWorkflowObject>
{
    public TaskWorkflowObjectMapping()
    {
        this.Schema("app");
        this.Id(x => x.Id).GeneratedBy.GuidComb();
        this.Map(x => x.Status).Access.CamelCaseField();
        this.Map(x => x.PostProcessWork).Access.CamelCaseField();

        //this.Component(
        //    x => x.WorkflowInfo,
        //    m =>
        //    {
        //        m.Map(x => x.IsTerminated).Access.CamelCaseField();
        //        m.Map(x => x.IsFinished).Access.CamelCaseField();
        //    });
    }
}