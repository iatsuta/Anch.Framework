namespace Anch.Workflow.Domain.Definition;

public record WorkflowDefinitionIdentity(string Name)
{
    public override string ToString() => this.Name;
}