using System.Collections.Frozen;
using System.Collections.Immutable;

namespace SecuritySystem.DiTests.Services;

public record TestPermission(SecurityRole SecurityRole, FrozenDictionary<Type, ImmutableArray<Guid>> Restrictions);