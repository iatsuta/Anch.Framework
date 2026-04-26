using System.Globalization;

namespace Anch.Core;

public interface ICultureSource
{
	CultureInfo Culture { get; }
}