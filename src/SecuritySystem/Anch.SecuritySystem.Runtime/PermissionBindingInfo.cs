using System.Linq.Expressions;

using Anch.Core;

namespace Anch.SecuritySystem;

/// <summary>Describes how a permission entity is bound to its principal, role and restrictions.</summary>
public abstract record PermissionBindingInfo
{
    public bool IsReadonly { get; init; }

    public abstract Type PrincipalType { get; }

    public abstract Type PermissionType { get; }
}

/// <summary>Describes how a permission entity is bound to its principal, role and restrictions.</summary>
public abstract record PermissionBindingInfo<TPermission> : PermissionBindingInfo
{
    public sealed override Type PermissionType { get; } = typeof(TPermission);

    public PropertyAccessors<TPermission, string>? PermissionComment { get; init; }

    public PropertyAccessors<TPermission, TPermission?>? DelegatedFrom { get; init; }

    public PropertyAccessors<TPermission, DateTime?>? PermissionStartDate { get; init; }

    public PropertyAccessors<TPermission, DateTime?>? PermissionEndDate { get; init; }

    public PermissionPeriod GetSafePeriod(TPermission permission)
    {
        return new PermissionPeriod(
            this.PermissionStartDate?.Getter.Invoke(permission),
            this.PermissionEndDate?.Getter.Invoke(permission));
    }

    public Expression<Func<TPermission, bool>> GetPeriodFilter(DateTime today) =>
        this.GetStartDateFilter(today).BuildAnd(this.GetEndDateFilter(today));

    private Expression<Func<TPermission, bool>> GetStartDateFilter(DateTime today)
    {
        return this.PermissionStartDate is null ? _ => true : this.PermissionStartDate.Path.Select(startDate => startDate == null || startDate <= today);
    }

    private Expression<Func<TPermission, bool>> GetEndDateFilter(DateTime today)
    {
        return this.PermissionEndDate is null ? _ => true : this.PermissionEndDate.Path.Select(endDate => endDate == null || endDate >= today);
    }


    public string GetSafeComment(TPermission permission) =>
        this.PermissionComment is null ? "" : this.PermissionComment.Getter(permission);
}

/// <summary>Describes how a permission entity is bound to its principal, role and restrictions.</summary>
public record PermissionBindingInfo<TPermission, TPrincipal> : PermissionBindingInfo<TPermission>
{
    public sealed override Type PrincipalType { get; } = typeof(TPrincipal);

    public required PropertyAccessors<TPermission, TPrincipal> Principal { get; init; }
}