using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Linq.Expressions;

namespace SyncWorkflow.Definition;

public interface IWorkflowDefinition
{
    WorkflowDefinitionIdentity Identity { get; }

    Type SourceType { get; }

    long Version => 1;

    LambdaExpression? StatusProperty { get; }

    LambdaExpression? VersionProperty { get; }

    bool InTechnical { get; }

    ImmutableArray<IStateDefinition> States { get; }

    IStateDefinition StartState { get; }

    IStateDefinition DefaultFinalState { get; }

    IStateDefinition TerminateState { get; }

    FrozenDictionary<string, object> Settings { get; }
}