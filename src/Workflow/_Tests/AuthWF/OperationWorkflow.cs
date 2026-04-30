using SyncWorkflow.Builder;

namespace AuthWF
{
    public class OperationWorkflow : BuildWorkflow<OperationWorkflowObject>
    {
        protected override void Build(IWorkflowBuilder<OperationWorkflowObject> builder)
        {
            builder.If<IAuthService>((wfObj, service) => service.CanAutoApprove(wfObj.Permission, wfObj.Operation),
                trueBuilder => trueBuilder.Finish());
        }
    }
}