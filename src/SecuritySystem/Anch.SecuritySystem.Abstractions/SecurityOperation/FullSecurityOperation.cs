

// ReSharper disable once CheckNamespace
namespace Anch.SecuritySystem;

/// <summary>An operation together with its registration data.</summary>
/// <param name="SecurityOperation">The operation.</param>
/// <param name="Info">Its registration data.</param>
public record FullSecurityOperation(SecurityOperation SecurityOperation, SecurityOperationInfo Info);
