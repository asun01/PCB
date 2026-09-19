namespace Asun.UI.Viewports;

public readonly record struct ViewportRenderEvidenceComparison(
    bool IsEquivalent,
    IReadOnlyList<string> Differences)
{
    public static ViewportRenderEvidenceComparison Equivalent { get; } =
        new(true, Array.Empty<string>());
}

public static class ViewportRenderEvidenceComparatorRuntime
{
    public static ViewportRenderEvidenceComparison Compare(
        ViewportRenderEvidenceManifest expected,
        ViewportRenderEvidenceManifest actual)
    {
        var differences = new List<string>();

        CompareValue(
            differences,
            nameof(ViewportRenderEvidenceManifest.Generation),
            expected.Generation,
            actual.Generation);

        CompareValue(
            differences,
            nameof(ViewportRenderEvidenceManifest.SubmissionSequence),
            expected.SubmissionSequence,
            actual.SubmissionSequence);

        CompareValue(
            differences,
            nameof(ViewportRenderEvidenceManifest.DirtyFlags),
            expected.DirtyFlags,
            actual.DirtyFlags);

        CompareValue(
            differences,
            nameof(ViewportRenderEvidenceManifest.BatchItemCount),
            expected.BatchItemCount,
            actual.BatchItemCount);

        CompareValue(
            differences,
            nameof(ViewportRenderEvidenceManifest.RegionCount),
            expected.RegionCount,
            actual.RegionCount);

        CompareValue(
            differences,
            nameof(ViewportRenderEvidenceManifest.TileCount),
            expected.TileCount,
            actual.TileCount);

        CompareValue(
            differences,
            nameof(ViewportRenderEvidenceManifest.RoiCount),
            expected.RoiCount,
            actual.RoiCount);

        CompareValue(
            differences,
            nameof(ViewportRenderEvidenceManifest.OverlayCount),
            expected.OverlayCount,
            actual.OverlayCount);

        CompareValue(
            differences,
            nameof(ViewportRenderEvidenceManifest.InvalidationCount),
            expected.InvalidationCount,
            actual.InvalidationCount);

        CompareValue(
            differences,
            nameof(ViewportRenderEvidenceManifest.FullSurfaceCount),
            expected.FullSurfaceCount,
            actual.FullSurfaceCount);

        CompareValue(
            differences,
            nameof(ViewportRenderEvidenceManifest.PlannedUnits),
            expected.PlannedUnits,
            actual.PlannedUnits);

        CompareValue(
            differences,
            nameof(ViewportRenderEvidenceManifest.RenderedUnits),
            expected.RenderedUnits,
            actual.RenderedUnits);

        CompareValue(
            differences,
            nameof(ViewportRenderEvidenceManifest.DeferredUnits),
            expected.DeferredUnits,
            actual.DeferredUnits);

        CompareValue(
            differences,
            nameof(ViewportRenderEvidenceManifest.BatchHash),
            expected.BatchHash,
            actual.BatchHash);

        CompareValue(
            differences,
            nameof(ViewportRenderEvidenceManifest.CommandHash),
            expected.CommandHash,
            actual.CommandHash);

        CompareValue(
            differences,
            nameof(ViewportRenderEvidenceManifest.FrameHash),
            expected.FrameHash,
            actual.FrameHash);

        CompareValue(
            differences,
            nameof(ViewportRenderEvidenceManifest.ReplayHash),
            expected.ReplayHash,
            actual.ReplayHash);

        return differences.Count == 0
            ? ViewportRenderEvidenceComparison.Equivalent
            : new ViewportRenderEvidenceComparison(false, differences);
    }

    public static bool AreEquivalent(
        ViewportRenderEvidenceManifest expected,
        ViewportRenderEvidenceManifest actual) =>
        Compare(expected, actual).IsEquivalent;

    private static void CompareValue<T>(
        ICollection<string> differences,
        string name,
        T expected,
        T actual)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
        {
            differences.Add(
                $"{name}: expected '{expected}', actual '{actual}'.");
        }
    }
}
