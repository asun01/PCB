namespace Asun.UI.Viewports;

public static class TilePrefetchValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        IReadOnlyList<TileRequest> source,
        IReadOnlyList<TileRequest> result,
        TilePrefetchPolicy policy)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(result);

        var errors = new List<string>();

        if (policy.MarginTiles < 0)
            errors.Add("Prefetch margin must be non-negative.");

        if (policy.MaximumTiles <= 0)
            errors.Add("Prefetch maximum tiles must be positive.");

        if (result.Count > Math.Max(0, policy.MaximumTiles))
            errors.Add("Prefetch result exceeds maximum tiles.");

        var sourceSet = source.ToHashSet();
        foreach (var item in result)
        {
            if (!sourceSet.Contains(item))
                errors.Add("Prefetch result contains a request absent from the source.");

            if (!item.IsVisible && !item.IsPrefetch)
                errors.Add("Non-visible prefetch request must report IsPrefetch.");
        }

        var distinctIndices = result
            .Select(item => item.Index)
            .Distinct()
            .Count();

        if (distinctIndices != result.Count)
            errors.Add("Prefetch result must not duplicate tile indices.");

        var visiblePrefixLength = result
            .TakeWhile(item => item.IsVisible)
            .Count();

        if (result
            .Skip(visiblePrefixLength)
            .Any(item => item.IsVisible))
        {
            errors.Add("Visible requests must remain before prefetch requests.");
        }

        return errors;
    }

    public static bool IsValid(
        IReadOnlyList<TileRequest> source,
        IReadOnlyList<TileRequest> result,
        TilePrefetchPolicy policy) =>
        Validate(source, result, policy).Count == 0;
}
