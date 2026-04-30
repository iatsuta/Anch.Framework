namespace AuthWF;

public interface IAuthService
{
    bool CanAutoApprove(Permission permission, Operation operation);

    PermissionWorkflowObject GetPermissionWorkflowObject(Permission permission);

    IEnumerable<OperationWorkflowObject> GetOperationWorkflowObjects(Permission permission);
}