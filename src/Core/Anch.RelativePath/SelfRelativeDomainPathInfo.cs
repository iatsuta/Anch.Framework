namespace Anch.RelativePath;

/// <summary>A relative path from a type to itself.</summary>
public record SelfRelativeDomainPathInfo<T>() : SingleRelativeDomainPathInfo<T, T>(v => v);
