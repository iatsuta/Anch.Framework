using Microsoft.Extensions.DependencyInjection;

using NHibernate;

using SyncWorkflow.Tests.StartWorkflowsWithTaskApprove;

namespace SyncWorkflow.Tests;

public class StartWorkflowsWithForksApproveTests : InlineWorkflowTestBase
{
    [Fact]
    public async Task Task_SendApproveCommand_WorkflowApproved()
    {
        //Arrange
        var sourceId = await this.EvaluateScope(async scope =>
        {
            var session = scope.ServiceProvider.GetRequiredService<ISession>();

            var source = new StartWorkflowsWithTaskApproveWorkflowObject
            {
                Items = Enumerable.Range(0, 5).Select(value => new StartWorkflowsWithTaskApproveItemWorkflowObject { Name = "Item" + value }).ToList()
            };

            await session.SaveAsync(source);

            return source.Id;
        });

        ////Act
        //var (rootWiIdentity, firstWiStatus, firstSiIdentity, startSourceStatus) = await this.EvaluateScope(async scope =>
        //{
        //    var session = scope.ServiceProvider.GetRequiredService<ISession>();

        //    var source = await session.GetAsync<StartWorkflowsWithTaskApproveWorkflowObject>(sourceId);

        //    var wi = await scope.StartWorkflow<StartWorkflowsWithTaskApproveWorkflowObject, StartWorkflowsWithTaskApproveWorkflow>(source);

        //    return (wi.Identity, wi.Status, wi.CurrentState.Identity, source.Status);
        //});

        //var processedWiIdents = await this.EvaluateScope(async scope =>
        //{
        //    var firstSi = await scope.Storage.GetStateInstance(firstSiIdentity);

        //    var pushResult = await scope.Host.PushEvent(TaskWorkflow.ApproveEventHeader, firstSi);

        //    return pushResult;
        //});

        ////Assert
        //await this.EvaluateAssertScope(async scope =>
        //{
        //    var wi = await scope.Storage.GetWorkflowInstance(rootWiIdentity);
        //    var source = (TaskWorkflowObject)wi.Source;

        //    source.PostProcessWork.Should().BeTrue();

        //    startSourceStatus.Should().Be(TaskApproveStatus.Approving);
        //    source.Status.Should().Be(TaskApproveStatus.Approved);

        //    firstWiStatus.Should().Be(WorkflowStatus.WaitEvent);
        //    wi.Status.Should().Be(WorkflowStatus.Finished);

        //    processedWiIdents.Count.Should().Be(1);
        //    processedWiIdents[0].Identity.Should().Be(rootWiIdentity);
        //});
    }

    //protected override IServiceCollection CreateServices()
    //{
    //    return base.CreateServices()

    //        .RegisterSyncWorkflowType<TaskWorkflow>()

    //        //.AddScoped<InlineWorkflowStorageItem<TaskWorkflowObject>, TaskWorkflowInlineWorkflowStorageItem>()
    //        .AddScoped<IWorkflowInstanceSerializer<TaskWorkflowObject>, TaskWorkflowWorkflowSerializer>()
    //        .AddScoped<IInlineStorage<TaskWorkflowObject>, TaskWorkflowInlineStorage>();
    //}
}

//public class TaskWorkflowWorkflowSerializer : IWorkflowInstanceSerializer<TaskWorkflowObject>
//{
//    private readonly IWorkflowHost host;

//    private readonly IWorkflow<TaskWorkflowObject> workflow;

//    public TaskWorkflowWorkflowSerializer(IWorkflowHost host, TaskWorkflow workflow)
//    {
//        this.host = host;
//        this.workflow = workflow;
//    }

//    public WorkflowInstance Deserialize(TaskWorkflowObject source)
//    {
//        var isTerminated = source.WorkflowInfo.IsTerminated;
//        var isFinished = source.WorkflowInfo.IsFinished;

//        var isTerminatedOrFinished = isTerminated || isFinished;

//        var currentStateDefinition =

//            isTerminated ? this.workflow.Definition.TerminateState
//            : isFinished ? this.workflow.Definition.FinalState
//            : this.workflow.Definition.States.Single(s => s.Name == source.Status.ToString());

//        var result = new WorkflowInstance
//        {
//            Definition = this.workflow.Definition,
//            Source = source,
//            Id = source.Id,
//        };

//        result.CurrentState = new StateInstance
//        {
//            Id = source.Id,
//            Workflow = result,
//            Definition = currentStateDefinition,
//            InputProcessed = true,
//            OutputProcessed = isTerminatedOrFinished,
//        };

//        if (currentStateDefinition.StateType == typeof(TaskState)
//            && currentStateDefinition.AdditionalInfo.TryGetValue(TaskState.AdditionalKey, out var untypedEventHeaders)
//            && untypedEventHeaders is IReadOnlyList<EventHeader> eventHeaders)
//        {
//            foreach (var eventHeader in eventHeaders)
//            {
//                result.CurrentState.WaitEvents.Add(new WaitEventInfo(eventHeader, null, result.CurrentState));
//            }

//            result.Status = WorkflowStatus.WaitEvent;
//        }
//        else if (isTerminated)
//        {
//            result.Status = WorkflowStatus.Terminated;
//        }
//        else if (isFinished)
//        {
//            result.Status = WorkflowStatus.Finished;
//        }
//        else
//        {
//            result.Status = WorkflowStatus.Runnable;
//        }

//        return result;
//    }

//    public void Serialize(WorkflowInstance wi)
//    {
//        var source = (TaskWorkflowObject)wi.Source;

//        source.WorkflowInfo = new ()
//        {
//            IsTerminated = wi.Status == WorkflowStatus.Terminated,
//            IsFinished = wi.Status == WorkflowStatus.Finished,
//        };

//        if (Enum.TryParse<TaskApproveStatus>(wi.CurrentState.Definition.Name, out var status))
//        {
//            source.Status = status;
//        }
//    }
//}

//public class TaskWorkflowInlineStorage : IInlineStorage<TaskWorkflowObject>
//{
//    private readonly ISession session;

//    public TaskWorkflowInlineStorage(ISession session)
//    {
//        this.session = session;
//    }

//    public async Task Save(TaskWorkflowObject source, CancellationToken cancellationToken = default)
//    {
//        await this.session.SaveAsync(source, cancellationToken);
//    }

//    public async Task FlushChanges(CancellationToken cancellationToken = default)
//    {
//        await this.session.FlushAsync(cancellationToken);
//    }

//    public async Task<IQueryable<TaskWorkflowObject>> GetQueryable(CancellationToken cancellationToken = default)
//    {
//        return this.session.Query<TaskWorkflowObject>();
//    }

//    public Expression<Func<TaskWorkflowObject, bool>> GetFilter(WorkflowInstanceIdentity wi)
//    {
//        return wfObj => wfObj.Id == wi.Id;
//    }

//    public Expression<Func<TaskWorkflowObject, bool>> GetFilter(StateInstanceIdentity si)
//    {
//        return wfObj => wfObj.Id == si.Id;
//    }
//}