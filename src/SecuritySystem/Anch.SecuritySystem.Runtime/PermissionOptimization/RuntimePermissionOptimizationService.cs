namespace Anch.SecuritySystem.PermissionOptimization;

public class RuntimePermissionOptimizationService : IRuntimePermissionOptimizationService
{
    public IEnumerable<Dictionary<Type, Array>> Optimize(IEnumerable<IReadOnlyDictionary<Type, Array>> permissions)
    {
        // An empty restriction array means "no restriction on this context" — access is granted for
        // any value of that context, which is equivalent to the context being absent. Drop such
        // entries and convert every permission to a single value-set representation up front, so the
        // rest of the algorithm works on one uniform shape.
        var current = permissions
            .Select(permission => permission
                .Where(pair => pair.Value.Length > 0)
                .ToDictionary(pair => pair.Key, pair => pair.Value.Cast<object>().ToHashSet()))
            .ToList();

        var byType = current.SelectMany(permission => permission).GroupBy(pair => pair.Key).ToList();

        // No permission restricts anything → every permission is the unrestricted global one.
        if (byType.Count == 0)
            return current.Count > 0 ? [new Dictionary<Type, Array>()] : [];

        // The runtime element type of each context's values, used to rebuild typed arrays at the end.
        var elementTypes = byType.ToDictionary(g => g.Key, g => g.SelectMany(pair => pair.Value).First().GetType());

        // Process the context with the most distinct values first.
        var orderedTypes = byType
            .OrderByDescending(g => g.SelectMany(pair => pair.Value).Distinct().Count())
            .Select(g => g.Key)
            .ToList();

        foreach (var type in orderedTypes)
        {
            // Group permissions by their restrictions on every OTHER context; the grouped element is
            // the permission's value-set for the current type (null when it does not restrict it).
            var groupings = current
                .GroupBy(permission => new GroupKey(permission, type), permission => permission.GetValueOrDefault(type))
                .ToList();

            // Empty group key = "no restrictions on any other context"; a null element = "no
            // restriction on the current type". A permission that is both restricts nothing at all —
            // the global permission that grants full access and absorbs everything else.
            if (groupings.Any(g => g.Key.Equals(GroupKey.Empty) && g.Any(value => value is null)))
                return [new Dictionary<Type, Array>()];

            // Merge: permissions sharing the same other-context restrictions get their current-type
            // values unioned.
            var grouped = groupings.ToDictionary(
                g => g.Key,
                g => g.SelectMany(value => value ?? Enumerable.Empty<object>()).ToHashSet());

            // baseSet = current-type values granted with no other restriction. Any other group that
            // grants one of those values together with extra restrictions is redundant, so strip it.
            if (grouped.Remove(GroupKey.Empty, out var baseSet))
            {
                foreach (var emptied in RefineGroupedPermissions(grouped, baseSet))
                    grouped.Remove(emptied);

                grouped[GroupKey.Empty] = baseSet;
            }

            current = grouped
                .Select(g => g.Key.GetKeyPairs()
                    .Concat(g.Value.Count > 0
                        ? [KeyValuePair.Create(type, g.Value)]
                        : Array.Empty<KeyValuePair<Type, HashSet<object>>>())
                    .ToDictionary(pair => pair.Key, pair => pair.Value))
                .ToList();
        }

        return current.Select(permission => permission.ToDictionary(
            pair => pair.Key,
            pair =>
            {
                var array = Array.CreateInstance(elementTypes[pair.Key], pair.Value.Count);
                pair.Value.ToArray().CopyTo(array, 0);
                return array;
            }));
    }

    private static List<GroupKey> RefineGroupedPermissions(
        Dictionary<GroupKey, HashSet<object>> grouped,
        HashSet<object> removeItems)
    {
        var removed = new List<GroupKey>();
        foreach (var pair in grouped)
        {
            if (pair.Value.Count == 0)
                continue;

            pair.Value.ExceptWith(removeItems);
            if (pair.Value.Count == 0)
                removed.Add(pair.Key);
        }
        return removed;
    }

    private sealed class GroupKey : IEquatable<GroupKey>
    {
        private readonly int hashCode;
        private readonly Dictionary<Type, HashSet<object>> keyData;

        public static readonly GroupKey Empty = new(new Dictionary<Type, HashSet<object>>(), typeof(GroupKey));

        public GroupKey(Dictionary<Type, HashSet<object>> dataItem, Type excludedType)
        {
            this.keyData = dataItem
                .Where(pair => pair.Key != excludedType)
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            this.hashCode = this.CalculateHashCode();
        }

        public IEnumerable<KeyValuePair<Type, HashSet<object>>> GetKeyPairs() => this.keyData;

        public override int GetHashCode() => this.hashCode;

        public bool Equals(GroupKey? other) => other is not null && this.DataEquals(other);

        public override bool Equals(object? obj) => obj is GroupKey other && this.DataEquals(other);

        private int CalculateHashCode()
        {
            var result = 0;
            foreach (var pair in this.keyData)
            {
                result ^= pair.Key.GetHashCode();
                foreach (var value in pair.Value)
                    result ^= value.GetHashCode();
            }
            return result;
        }

        private bool DataEquals(GroupKey other)
        {
            if (this.keyData.Count != other.keyData.Count)
                return false;

            foreach (var pair in this.keyData)
            {
                if (!other.keyData.TryGetValue(pair.Key, out var otherSet) || !pair.Value.SetEquals(otherSet))
                    return false;
            }
            return true;
        }
    }
}
