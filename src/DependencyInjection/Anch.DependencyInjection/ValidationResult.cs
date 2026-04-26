using System.Collections.Immutable;

namespace Anch.DependencyInjection;

/// <summary>
/// Represents the result of validating a collection of services or other entities.
/// </summary>
/// <remarks>
/// This type is intended for use with DI container validators, where a validation
/// may produce multiple errors. Each error is represented as a string in the <see cref="Errors"/> list.
/// </remarks>
public record ValidationResult(ImmutableArray<string> Errors)
{
    /// <summary>
    /// Initializes a new instance of <see cref="ValidationResult"/> from an enumerable of error messages.
    /// </summary>
    /// <param name="errors">A sequence of error messages produced by the validation.</param>
    public ValidationResult(IEnumerable<string> errors)
        : this([..errors])
    {
    }

    /// <summary>
    /// Gets a value indicating whether the validation was successful (i.e., no errors were reported).
    /// </summary>
    public bool IsSuccess => this.Errors.Length == 0;

    /// <summary>
    /// Gets a shared <see cref="ValidationResult"/> instance representing a successful validation.
    /// </summary>
    public static ValidationResult Success { get; } = new([]);

    /// <summary>
    /// Combines two <see cref="ValidationResult"/> instances into a single result, concatenating their error lists.
    /// </summary>
    /// <param name="left">The first validation result.</param>
    /// <param name="right">The second validation result.</param>
    /// <returns>A new <see cref="ValidationResult"/> containing all errors from both inputs.</returns>
    public static ValidationResult operator +(ValidationResult left, ValidationResult right)
    {
        return new ValidationResult(left.Errors.Concat(right.Errors));
    }
}
