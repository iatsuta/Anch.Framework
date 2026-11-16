namespace Anch.Core;

/// <summary>A value type with a single value, used where a generic parameter is required but no value matters (like <c>Unit</c> or <c>void</c>).</summary>
public record struct Ignore
{
    /// <summary>The only value.</summary>
    public static readonly Ignore Value = new();
}