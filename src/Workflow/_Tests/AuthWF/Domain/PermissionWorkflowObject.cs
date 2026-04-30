namespace AuthWF;

public class PermissionWorkflowObject
{
    public Permission Permission { get; set; }

    public List<OperationWorkflowObject> Operations { get; set; }
}
