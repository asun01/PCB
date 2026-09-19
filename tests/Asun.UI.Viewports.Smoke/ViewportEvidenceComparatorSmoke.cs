using Asun.UI.Viewports;

public static class ViewportEvidenceComparatorSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var expected = new ViewportRenderEvidenceManifest(
            Generation: 10,
            SubmissionSequence: 20,
            DirtyFlags: ViewportDirtyFlags.Image,
            BatchItemCount: 2,
            RegionCount: 1,
            TileCount: 1,
            RoiCount: 1,
            OverlayCount: 0,
            InvalidationCount: 1,
            FullSurfaceCount: 0,
            PlannedUnits: 2,
            RenderedUnits: 2,
            DeferredUnits: 0,
            BatchHash: new string('a', 64),
            CommandHash: new string('b', 64),
            FrameHash: new string('c', 64),
            ReplayHash: new string('d', 64));

        var identical = ViewportRenderEvidenceComparatorRuntime.Compare(
            expected,
            expected);

        assert(
            identical.IsEquivalent &&
            identical.Differences.Count == 0 &&
            ViewportRenderEvidenceComparatorRuntime.AreEquivalent(expected, expected),
            "Equivalent evidence manifests should compare equal with no differences.");

        var changed = expected with
        {
            RenderedUnits = 1,
            DeferredUnits = 1,
            FrameHash = new string('e', 64)
        };

        var comparison = ViewportRenderEvidenceComparatorRuntime.Compare(
            expected,
            changed);

        assert(
            !comparison.IsEquivalent &&
            comparison.Differences.Count == 3 &&
            comparison.Differences.Any(
                difference => difference.StartsWith(
                    "RenderedUnits:",
                    StringComparison.Ordinal)) &&
            comparison.Differences.Any(
                difference => difference.StartsWith(
                    "DeferredUnits:",
                    StringComparison.Ordinal)) &&
            comparison.Differences.Any(
                difference => difference.StartsWith(
                    "FrameHash:",
                    StringComparison.Ordinal)),
            "Evidence comparison should report each changed logical field deterministically.");

        var restored = changed with
        {
            RenderedUnits = expected.RenderedUnits,
            DeferredUnits = expected.DeferredUnits,
            FrameHash = expected.FrameHash
        };

        assert(
            ViewportRenderEvidenceComparatorRuntime.AreEquivalent(
                expected,
                restored),
            "Restoring all changed fields should restore evidence equivalence.");
    }
}
