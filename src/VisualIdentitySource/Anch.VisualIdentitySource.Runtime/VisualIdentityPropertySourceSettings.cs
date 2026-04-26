using System.Collections.Immutable;

namespace Anch.VisualIdentitySource;

public record VisualIdentityPropertySourceSettings(ImmutableArray<string> PropertyNameList)
{
	public static VisualIdentityPropertySourceSettings Default { get; } = new(["Login", "Name", "Code"]);
}