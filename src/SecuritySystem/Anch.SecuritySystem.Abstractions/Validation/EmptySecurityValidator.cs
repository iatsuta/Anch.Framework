namespace Anch.SecuritySystem.Validation;

/// <summary>A validator that accepts everything.</summary>
/// <typeparam name="T">The type of the validated value.</typeparam>
public class EmptySecurityValidator<T> : ISecurityValidator<T>
{
    /// <inheritdoc/>
    public ValueTask ValidateAsync(T value, CancellationToken ct) => ValueTask.CompletedTask;
}