using System.Globalization;

namespace CommonFramework;

public interface ICultureSource
{
	CultureInfo Culture { get; }
}