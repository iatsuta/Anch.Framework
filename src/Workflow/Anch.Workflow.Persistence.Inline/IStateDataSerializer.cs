using Anch.Workflow.Domain.Runtime;

namespace Anch.Workflow.Persistence.Inline;

/// <summary>
/// Maps the runtime data of a state instance to and from strictly typed members of the domain object.
/// </summary>
/// <typeparam name="TSource">The domain object that stores the state inline.</typeparam>
public interface IStateDataSerializer<in TSource>
    where TSource : class
{
    /// <summary>
    /// Rebuilds the runtime data (wait events, children, ...) of <paramref name="stateInstance"/> from the domain object.
    /// </summary>
    /// <param name="source">The domain object.</param>
    /// <param name="stateInstance">The state instance being restored.</param>
    void Restore(TSource source, StateInstance stateInstance);

    /// <summary>
    /// Writes the runtime data of <paramref name="stateInstance"/> into the typed members of the domain object.
    /// </summary>
    /// <param name="source">The domain object.</param>
    /// <param name="stateInstance">The state instance being persisted.</param>
    void Store(TSource source, StateInstance stateInstance);
}

/// <summary>
/// A <see cref="IStateDataSerializer{TSource}"/> bound to one state type.
/// Register an implementation in DI to persist the data of a custom state in typed members of <typeparamref name="TSource"/>.
/// </summary>
/// <typeparam name="TSource">The domain object that stores the state inline.</typeparam>
/// <typeparam name="TState">The state type the serializer is responsible for.</typeparam>
public interface IStateDataSerializer<in TSource, TState> : IStateDataSerializer<TSource>
    where TSource : class;
