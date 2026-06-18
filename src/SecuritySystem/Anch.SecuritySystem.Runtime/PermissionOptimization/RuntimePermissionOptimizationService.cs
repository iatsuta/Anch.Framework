namespace Anch.SecuritySystem.PermissionOptimization;

public class RuntimePermissionOptimizationService : IRuntimePermissionOptimizationService
{
    public IEnumerable<Dictionary<Type, Array>> Optimize(IEnumerable<IReadOnlyDictionary<Type, Array>> permissions)
    {
        // An empty restriction array means "no restriction on this context" — access is granted for
        // any value of that context, which is equivalent to the context being absent from the
        // permission. Drop such entries so the rest of the algorithm only deals with real
        // (non-empty) restrictions.
        var cachedPermissions = permissions
            .Select(permission => permission
                .Where(pair => pair.Value.Length > 0)
                .ToDictionary(pair => pair.Key, pair => pair.Value))
            .ToList();

        var orderedTypes = cachedPermissions
            .SelectMany(p => p)
            .GroupBy(p => p.Key)
            .OrderByDescending(g => g.SelectMany(p => p.Value.Cast<object>()).Distinct().Count())
            .Select(g => g.Key)
            .ToList();

        // No permission restricts anything → every permission is the unrestricted global one.
        if (orderedTypes.Count == 0)
            return cachedPermissions.Count > 0 ? [new Dictionary<Type, Array>()] : [];

        IEnumerable<Dictionary<Type, HashSet<object>>>? current = null;

        foreach (var type in orderedTypes)
        {
            var groupings = GetGroupable(current, cachedPermissions, type)
                .GroupBy(item => item.Key)
                .ToList();

            // The empty group key means "no restrictions on any other context"; a null value means
            // "no restriction on the current type". A permission that is both restricts nothing at
            // all — the global permission. It grants full access and absorbs every other permission,
            // so the result collapses to a single unrestricted permission.
            if (groupings.Any(g => g.Key.Equals(GroupKey.Empty) && g.Any(item => item.Value is null)))
                return [new Dictionary<Type, Array>()];

            var grouped = groupings.ToDictionary(
                g => g.Key,
                g => g.SelectMany(i => i.Value ?? Enumerable.Empty<object>())
                      .ToHashSet()
            );

            if (grouped.TryGetValue(GroupKey.Empty, out var baseSet))
            {
                grouped.Remove(GroupKey.Empty);

                var toRemove = RefineGroupedPermissions(grouped, baseSet);
                foreach (var k in toRemove)
                    grouped.Remove(k);

                grouped[GroupKey.Empty] = baseSet;
            }

            current = grouped.Select(g =>
                g.Key.GetKeyPairs()
                     .Concat(g.Value.Count > 0 ? [KeyValuePair.Create(type, g.Value)] : Array.Empty<KeyValuePair<Type, HashSet<object>>>())
                     .ToDictionary(p => p.Key, p => p.Value)
            );
        }

        return current?.Select(d => d.ToDictionary(
            p => p.Key,
            p =>
            {
                var elementType = GetElementType(d, p.Key);
                var arr = Array.CreateInstance(elementType, p.Value.Count);
                p.Value.ToArray().CopyTo(arr, 0);
                return arr;
            }))
            ?? [];
    }


    private static IEnumerable<GroupableItem> GetGroupable(
        IEnumerable<Dictionary<Type, HashSet<object>>>? main,
        IEnumerable<Dictionary<Type, Array>> additional,
        Type currentType) =>
        main?.Select(p => new GroupableItem(new GroupKey(p, currentType), p.ContainsKey(currentType) ? p[currentType] : null))
        ?? additional.Select(p => new GroupableItem(new GroupKey(p, currentType), p.ContainsKey(currentType) ? p[currentType].Cast<object>().ToHashSet() : null));

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

    private static Type GetElementType(Dictionary<Type, HashSet<object>> d, Type key)
    {
        if (d.TryGetValue(key, out var set) && set.Count > 0)
            return set.First().GetType();
        return typeof(object);
    }

    private record GroupableItem(GroupKey Key, HashSet<object>? Value);

    private sealed class GroupKey : IEquatable<GroupKey>
    {
        private readonly int hashCode;
        private readonly IDictionary<Type, HashSet<object>> keyData;

        public static readonly GroupKey Empty = new(new Dictionary<Type, Array>(), typeof(GroupKey));

        public GroupKey(Dictionary<Type, Array> dataItem, Type excludedType)
        {
            this.keyData = new Dictionary<Type, HashSet<object>>(dataItem.Count);
            foreach (var pair in dataItem)
            {
                if (pair.Key == excludedType)
                    continue;

                var set = new HashSet<object>();
                foreach (var el in pair.Value)
                    set.Add(el!);

                this.keyData.Add(pair.Key, set);
            }

            this.hashCode = this.CalculateHashCode();
        }

        public GroupKey(Dictionary<Type, HashSet<object>> dataItem, Type excludedType)
        {
            this.keyData = new Dictionary<Type, HashSet<object>>(dataItem.Where(pair => pair.Key != excludedType));
            this.hashCode = this.CalculateHashCode();
        }

        public IEnumerable<KeyValuePair<Type, HashSet<object>>> GetKeyPairs() => this.keyData;
        public override int GetHashCode() => this.hashCode;

        public bool Equals(GroupKey? other) => this.Equals((object?)other);

        public override bool Equals(object? obj) =>
            obj is GroupKey gk && this.DataEquals(gk);

        private int CalculateHashCode()
        {
            var result = 0;
            foreach (var pair in this.keyData)
            {
                result ^= pair.Key.GetHashCode();
                foreach (var val in pair.Value)
                {
                    result ^= val.GetHashCode();
                }
            }
            return result;
        }

        private bool DataEquals(GroupKey other)
        {
            if (this.keyData.Count != other.keyData.Count)
                return false;

            foreach (var pair in this.keyData)
            {
                if (!other.keyData.TryGetValue(pair.Key, out var otherSet))
                    return false;

                if (!pair.Value.SetEquals(otherSet))
                    return false;
            }
            return true;
        }
    }
}