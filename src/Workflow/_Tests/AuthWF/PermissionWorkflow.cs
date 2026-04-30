using SyncWorkflow;
using SyncWorkflow.Builder;
using SyncWorkflow.States.Output;

namespace AuthWF;

public class PermissionWorkflow : BuildWorkflow<PermissionWorkflowObject>
{
    protected override void Build(IWorkflowBuilder<PermissionWorkflowObject> builder)
    {
        builder.Then<WriteLineState>()
            .Input(state => state.Message, "Hello, World!");

        //builder
        //    .Foreach(
        //        (PermissionWorkflowObject wfObj, IAuthService authService) => authService.GetOperationWorkflowObjects(wfObj.Permission),
        //        true,
        //        iteratorBuilder => iteratorBuilder.StartWorkflow<OperationWorkflow>())

            // .Then<IfState<Permission>()
           //     .Input(state => state.Condition, _ => true)




            ;
    }
}