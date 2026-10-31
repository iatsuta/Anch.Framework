using System.Collections.Immutable;

namespace Anch.SecuritySystem.SecurityAccessor;

/// <summary>Describes who has access to an object: a fixed set of users, everyone, nobody, or a combination.</summary>
public abstract record SecurityAccessorData
{
    /// <summary>Everyone has access.</summary>
    public static SecurityAccessorData Infinity { get; } = new InfinitySecurityAccessorData();

    /// <summary>Nobody has access.</summary>
    public static SecurityAccessorData Empty { get; } = Return();

    /// <summary>Exactly these users have access.</summary>
    /// <param name="items">The user names.</param>
    public static SecurityAccessorData Return(params string[] items) => Return(items.ToImmutableArray());

    /// <summary>Exactly these users have access.</summary>
    /// <param name="items">The user names.</param>
    public static SecurityAccessorData Return(IEnumerable<string> items) => Return(items.ToImmutableArray());

    /// <summary>Exactly these users have access.</summary>
    /// <param name="items">The user names.</param>
    public static SecurityAccessorData Return(ImmutableArray<string> items) => new FixedSecurityAccessorData(items);

    /// <summary>The given user has access; nobody if the name is empty.</summary>
    /// <param name="item">The user name, possibly empty.</param>
    public static SecurityAccessorData TryReturn(string? item) => string.IsNullOrWhiteSpace(item) ? Empty : Return(item);

    /// <summary>A fixed set of users.</summary>
    /// <param name="Items">The user names.</param>
    public record FixedSecurityAccessorData(ImmutableArray<string> Items) : SecurityAccessorData;

    /// <summary>Everyone.</summary>
    public record InfinitySecurityAccessorData : SecurityAccessorData;

    /// <summary>The users that both parts allow.</summary>
    /// <param name="Left">The first part.</param>
    /// <param name="Right">The second part.</param>
    public record AndSecurityAccessorData(SecurityAccessorData Left, SecurityAccessorData Right) : SecurityAccessorData;

    /// <summary>The users that either part allows.</summary>
    /// <param name="Left">The first part.</param>
    /// <param name="Right">The second part.</param>
    public record OrSecurityAccessorData(SecurityAccessorData Left, SecurityAccessorData Right) : SecurityAccessorData;

    /// <summary>Everyone except the users that the inner data allows.</summary>
    /// <param name="InnerData">The data to invert.</param>
    public record NegateSecurityAccessorData(SecurityAccessorData InnerData) : SecurityAccessorData;
}
