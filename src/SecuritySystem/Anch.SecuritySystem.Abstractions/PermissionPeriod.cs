namespace Anch.SecuritySystem;

/// <summary>The validity period of a permission; an open end means "no limit".</summary>
/// <param name="StartDate">First day of validity; <c>null</c> for no lower limit.</param>
/// <param name="EndDate">Last day of validity; <c>null</c> for no upper limit.</param>
public record struct PermissionPeriod(DateTime? StartDate, DateTime? EndDate)
{
    /// <summary>Determines whether the periods overlap.</summary>
    /// <param name="otherPeriod">The period to compare with.</param>
    public bool IsIntersected(PermissionPeriod otherPeriod)
    {
        var start1 = this.StartDate ?? DateTime.MinValue;
        var end1 = this.EndDate ?? DateTime.MaxValue;

        var start2 = otherPeriod.StartDate ?? DateTime.MinValue;
        var end2 = otherPeriod.EndDate ?? DateTime.MaxValue;

        return start1 <= end2 && start2 <= end1;
    }

    /// <summary>Determines whether this period fully contains the other one.</summary>
    /// <param name="otherPeriod">The period to compare with.</param>
    public bool Contains(PermissionPeriod otherPeriod)
    {
        var start1 = this.StartDate ?? DateTime.MinValue;
        var end1 = this.EndDate ?? DateTime.MaxValue;

        var start2 = otherPeriod.StartDate ?? DateTime.MinValue;
        var end2 = otherPeriod.EndDate ?? DateTime.MaxValue;

        return start1 <= start2 && end2 <= end1;
    }

    /// <summary>A period without limits.</summary>
    public static PermissionPeriod Eternity { get; } = new(null, null);
}