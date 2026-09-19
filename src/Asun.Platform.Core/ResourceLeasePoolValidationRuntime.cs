namespace Asun.Platform.Core;

public static class ResourceLeasePoolValidationRuntime
{
    public static IReadOnlyList<string> Validate<TKey>(
        ResourceLeasePool<TKey> pool)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(pool);

        var errors = new List<string>();

        if (pool.ResourceCount <= 0)
            errors.Add("Resource pool must contain at least one resource.");

        foreach (var key in pool.ResourceKeys)
        {
            if (!pool.TryGetCapacity(key, out var capacity) ||
                capacity <= 0)
            {
                errors.Add("Configured resource capacity must be positive.");
                continue;
            }

            if (!pool.TryGetAvailable(key, out var available) ||
                available < 0 ||
                available > capacity)
            {
                errors.Add("Resource availability must stay within configured capacity.");
            }
        }

        return errors;
    }

    public static bool IsValid<TKey>(ResourceLeasePool<TKey> pool)
        where TKey : notnull =>
        Validate(pool).Count == 0;
}
