namespace Anch.SecuritySystem.Validation;

/// <summary>Marker of security validators.</summary>
public interface ISecurityValidator
{
    /// <summary>Key under which the individual validators that make up the composite are registered.</summary>
    public const string ElementKey = "Element";
}

/// <summary>Validates a value (for example a permission or a principal) before it is saved; throws <see cref="SecuritySystemValidationException"/> when it is invalid.</summary>
/// <typeparam name="T">The type of the validated value.</typeparam>
public interface ISecurityValidator<in T> : ISecurityValidator
{
    /// <summary>Validates the value.</summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="ct">Cancellation token.</param>
    ValueTask ValidateAsync(T value, CancellationToken ct);
}