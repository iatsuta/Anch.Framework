using Microsoft.Extensions.DependencyInjection;

namespace Anch.DependencyInjection;

/// <summary>
/// Provides extension methods for registering and executing <see cref="IServiceCollectionValidator"/> instances
/// on an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionValidationExtensions
{
    /// <param name="services">The service collection to add the validator to.</param>
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Registers a validator of type <typeparamref name="TValidator"/> with the service collection.
        /// </summary>
        /// <typeparam name="TValidator">The type of the validator to register. Must implement <see cref="IServiceCollectionValidator"/> and have a parameterless constructor.</typeparam>
        /// <returns>The original <see cref="IServiceCollection"/> for chaining.</returns>
        public IServiceCollection AddValidator<TValidator>()
            where TValidator : IServiceCollectionValidator, new()
        {
            return services.AddValidator(new TValidator());
        }

        /// <summary>
        /// Registers an instance of <see cref="IServiceCollectionValidator"/> with the service collection.
        /// </summary>
        /// <param name="validator">The validator instance to register.</param>
        /// <returns>The original <see cref="IServiceCollection"/> for chaining.</returns>
        public IServiceCollection AddValidator(IServiceCollectionValidator validator)
        {
            return services.AddSingleton(validator);
        }

        /// <summary>
        /// Performs manual validation of the <see cref="IServiceCollection"/> using all registered validators.
        /// </summary>
        /// <param name="options">Optional parameter providing additional options for validation. Can be <c>null</c>.</param>
        /// <returns>The original <see cref="IServiceCollection"/> for chaining.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if any of the registered validators report errors. The exception message contains all aggregated validation errors.
        /// </exception>
        /// <remarks>
        /// This method is a temporary workaround. The default DI container does not natively support
        /// collecting validation errors at build time. Ideally, validators would be invoked automatically
        /// during <c>BuildServiceProvider</c>, and all errors would be aggregated into a single exception
        /// thrown by the DI engine.
        /// </remarks>
        public IServiceCollection Validate(object? options = null)
        {
            var validationResult = services
                .GetValidators()
                .Select(validator => validator.Validate(services, options))
                .Aggregate(ValidationResult.Success, (v1, v2) => v1 + v2);

            if (!validationResult.IsSuccess)
            {
                var message = string.Join(Environment.NewLine, validationResult.Errors);

                throw new InvalidOperationException(message);
            }

            return services;
        }

        private IEnumerable<IServiceCollectionValidator> GetValidators()
        {
            return

                from sd in services

                where sd is { Lifetime: ServiceLifetime.Singleton, IsKeyedService: false } && sd.ServiceType == typeof(IServiceCollectionValidator)

                let validator = sd.ImplementationInstance as IServiceCollectionValidator

                where validator != null

                select validator;
        }
    }
}