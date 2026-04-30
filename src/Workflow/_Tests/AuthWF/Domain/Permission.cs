namespace AuthWF;

public class Permission
{
    public string Name { get; set; }

    public List<Operation> Operations { get; set; } = new List<Operation>();

}
