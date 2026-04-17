using System.Linq.Expressions;

namespace SyncWorkflow.Domain.Definition;

public interface IWorkflowDefinition
{
    WorkflowDefinitionIdentity Identity { get; }
    
    long Version => 1;

    LambdaExpression? IdProperty { get; }
    
    LambdaExpression? StatusProperty { get; }
    
    LambdaExpression? VersionProperty { get; }

    bool InTechnical { get; }

    Type SourceType { get; }

    IEnumerable<IStateDefinition> States { get; }

    IStateDefinition StartState { get; }

    IStateDefinition DefaultFinalState { get; }

    IStateDefinition TerminateState { get; }

    IReadOnlyDictionary<string, object> Settings { get; }

    public void Validate();
}




//public record InlinePropertyData(LambdaExpression IdProperty, LambdaExpression StatusProperty, LambdaExpression? VersionProperty = null)
//{

//}