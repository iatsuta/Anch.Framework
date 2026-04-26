using System.Globalization;

namespace Anch.Core;

public class CultureSource(CultureInfo culture) : ICultureSource
{
    public CultureInfo Culture { get; } = culture;

    public static CultureSource CurrentCulture { get; } = new(CultureInfo.CurrentCulture);
}