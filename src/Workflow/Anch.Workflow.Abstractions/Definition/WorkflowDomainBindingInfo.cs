using Anch.Core;

namespace Anch.Workflow.Definition;

public record WorkflowDomainBindingInfo<TSource, TStatus> : WorkflowDomainBindingInfo<TSource>
{
    public required PropertyAccessors<TSource, TStatus> Status { get; init; }

    public override Type? StatusType { get; } = typeof(TStatus);
}

public record WorkflowDomainBindingInfo<TSource> : WorkflowDomainBindingInfo
{
    public override Type SourceType { get; } = typeof(TSource);

    public PropertyAccessors<TSource, long>? Version { get; init; }
}

public abstract record WorkflowDomainBindingInfo
{
    public abstract Type SourceType { get; }

    public virtual Type? StatusType { get; } = null;
}