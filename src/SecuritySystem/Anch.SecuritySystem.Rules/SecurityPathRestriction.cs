using System.Linq.Expressions;

using Anch.Core;

namespace Anch.SecuritySystem;

/// <summary>
/// Describes additional restrictions on a SecurityPath; bound to a role.
/// Immutable: the <c>Add*</c> methods return a modified copy.
/// </summary>
/// <param name="SecurityContextRestrictions">Allowed security context types with their options; null means all context types are allowed</param>
/// <param name="ConditionFactoryTypes">Types of factories that produce additional filter conditions</param>
/// <param name="RelativeConditions">Additional conditions on the domain object itself</param>
/// <param name="IgnoreSecurityPath">Whether the SecurityPath is ignored</param>
public record SecurityPathRestriction(
    DeepEqualsCollection<SecurityContextRestriction>? SecurityContextRestrictions,
    DeepEqualsCollection<Type> ConditionFactoryTypes,
    DeepEqualsCollection<RelativeConditionInfo> RelativeConditions,
    bool IgnoreSecurityPath)
{
    private int? hashCode;

    protected SecurityPathRestriction(SecurityPathRestriction source)
    {
        this.SecurityContextRestrictions = source.SecurityContextRestrictions;
        this.ConditionFactoryTypes = source.ConditionFactoryTypes;
        this.RelativeConditions = source.RelativeConditions;
        this.IgnoreSecurityPath = source.IgnoreSecurityPath;
    }

    public IEnumerable<Type>? SecurityContextTypes => this.SecurityContextRestrictions?.Select(v => v.SecurityContextType);

    /// <summary>
    /// Default restrictions for roles (all context types are allowed, SecurityPath is applied).
    /// </summary>
    public static SecurityPathRestriction Default { get; } = new(null, Array.Empty<Type>(), Array.Empty<RelativeConditionInfo>(), false);

    /// <summary>
    /// Restrictions for the built-in 'Administrator' and 'SystemIntegration' roles (all contexts are denied and SecurityPath is not applied).
    /// </summary>
    public static SecurityPathRestriction Ignored { get; } = new(
        Array.Empty<SecurityContextRestriction>(),
        Array.Empty<Type>(),
        Array.Empty<RelativeConditionInfo>(),
        true);

    public SecurityPathRestriction Add<TSecurityContext>(
        bool required = false,
        string? key = null,
        Expression<Func<TSecurityContext, bool>>? filter = null)
        where TSecurityContext : ISecurityContext =>
        this.Add(
            required,
            key,
            filter is null
                ? null
                : ExpressionHelper.Create((IServiceProvider _) => filter));

    public SecurityPathRestriction Add<TSecurityContext, TFilterService>(
        bool required = false,
        string? key = null,
        Expression<Func<TFilterService, Expression<Func<TSecurityContext, bool>>>>? filter = null)
        where TSecurityContext : ISecurityContext
        where TFilterService : notnull =>
        this with
        {
            SecurityContextRestrictions =

            this.SecurityContextRestrictions
                .EmptyIfNull()
                .Concat(
                [
                    new SecurityContextRestriction<TSecurityContext>(
                        required,
                        key,
                        filter is null ? null : new SecurityContextRestrictionFilterInfo<TSecurityContext, TFilterService>(filter))
                ])
                .ToArray()
        };

    public SecurityPathRestriction AddRelativeCondition<TDomainObject>(Expression<Func<TDomainObject, bool>> condition) =>

        this with { RelativeConditions = this.RelativeConditions.Concat([new RelativeConditionInfo<TDomainObject>(condition)]).ToArray() };

    public SecurityPathRestriction AddConditionFactory(Type conditionFactoryType) =>

        this with { ConditionFactoryTypes = this.ConditionFactoryTypes.Concat([conditionFactoryType]).ToArray() };

    public static SecurityPathRestriction Create<TSecurityContext>(
        bool required = false,
        string? key = null,
        Expression<Func<TSecurityContext, bool>>? filter = null)
        where TSecurityContext : ISecurityContext => Default.Add(required, key, filter);

    public static SecurityPathRestriction Create<TSecurityContext, TFilterService>(
        bool required = false,
        string? key = null,
        Expression<Func<TFilterService, Expression<Func<TSecurityContext, bool>>>>? filter = null)
        where TSecurityContext : ISecurityContext
        where TFilterService : notnull => Default.Add(required, key, filter);

    public static SecurityPathRestriction Create<TDomainObject>(Expression<Func<TDomainObject, bool>> condition) =>
        Default.AddRelativeCondition(condition);

    public virtual bool Equals(SecurityPathRestriction? other) =>
        object.ReferenceEquals(this, other)
        || (other is not null
            && this.GetHashCode() == other.GetHashCode()
            && this.IgnoreSecurityPath == other.IgnoreSecurityPath
            && this.SecurityContextRestrictions == other.SecurityContextRestrictions
            && this.ConditionFactoryTypes == other.ConditionFactoryTypes
            && this.RelativeConditions == other.RelativeConditions);

    public override int GetHashCode() => this.hashCode ??=
        HashCode.Combine(this.SecurityContextRestrictions, this.ConditionFactoryTypes, this.RelativeConditions, this.IgnoreSecurityPath);
}