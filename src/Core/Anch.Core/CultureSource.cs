using System.Globalization;

namespace Anch.Core;

/// <summary>An <see cref="ICultureSource"/> with a fixed culture.</summary>
/// <param name="culture">The culture.</param>
public class CultureSource(CultureInfo culture) : ICultureSource
{
    /// <inheritdoc/>
    public CultureInfo Culture { get; } = culture;

    /// <summary>The invariant culture.</summary>
    public static CultureSource InvariantCulture { get; } = new(CultureInfo.InvariantCulture);
}