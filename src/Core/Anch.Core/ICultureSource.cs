using System.Globalization;

namespace Anch.Core;

/// <summary>Supplies the culture to use when parsing and formatting values (for example in the OData parser).</summary>
public interface ICultureSource
{
    /// <summary>The current culture.</summary>
    CultureInfo Culture { get; }
}