using Microsoft.Extensions.DependencyInjection;

namespace Anch.DependencyInjection;

/// <summary>
/// Represents a validator that inspects an <see cref="IServiceCollection"/>
/// and reports any issues found during validation.
/// </summary>
public interface IServiceCollectionValidator
{
    /// <summary>
    /// Validates the provided <see cref="IServiceCollection"/> and returns a <see cref="ValidationResult"/>
    /// containing any errors found.
    /// </summary>
    /// <param name="services">The service collection to validate.</param>
    /// <param name="options">
    /// Optional object containing additional options for the validation.
    /// Can be <c>null</c>.
    /// </param>
    /// <returns>A <see cref="ValidationResult"/> indicating success or containing validation errors.</returns>
    ValidationResult Validate(IServiceCollection services, object? options);
}