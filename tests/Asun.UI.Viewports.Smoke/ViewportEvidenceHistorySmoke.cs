using Asun.UI.Viewports;

public static class ViewportEvidenceHistorySmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var audit = new ViewportPresentationAuditTrace(capacity: 3);

        for (var index = 0; index < 5; index++)
        {
            audit.Record(
                stage: $"Stage-{index}",
                generation: index,
                submissionSequence: index + 1);
        }

        var auditEvents = audit.Snapshot();

        assert(
            audit.Capacity == 3 &&
            audit.Count == 3 &&
            audit.DroppedCount == 2 &&
            auditEvents[0].Stage == "Stage-2" &&
            auditEvents[^1].Stage == "Stage-4" &&
            audit.IsStrictlyOrdered(),
            "Audit history should remain bounded while preserving newest ordered events.");

        audit.Reset();

        assert(
            audit.Count == 0 &&
            audit.DroppedCount == 0,
            "Audit history reset should clear retained events and drop counters.");

        var store = new ViewportRenderEvidenceStore(capacity: 2);

        ViewportRenderEvidenceManifest Build(long generation, long sequence) =>
            new(
                generation,
                sequence,
                ViewportDirtyFlags.Image,
                1,
                1,
                1,
                0,
                0,
                0,
                0,
                1,
                1,
                0,
                new string((char)('a' + (int)generation), 64),
                new string((char)('b' + (int)generation), 64),
                new string((char)('c' + (int)generation), 64),
                "");

        var first = Build(1, 1);
        var second = Build(1, 2);
        var third = Build(2, 1);

        store.Add(first);
        store.Add(second);
        store.Add(third);

        assert(
            store.Count == 2 &&
            store.Latest == third &&
            store.Snapshot()[0] == second &&
            store.Validate().Count == 0,
            "Evidence history should retain only the newest monotonic manifests.");

        assert(
            store.TryGetByStableKey(
                third.StableKey,
                out var found) &&
            found == third,
            "Evidence history lookup should resolve the exact stable key.");

        assert(
            !store.TryGetByStableKey(
                first.StableKey,
                out _),
            "Evicted evidence should no longer be addressable in a bounded history.");

        var nonMonotonicRejected = false;

        try
        {
            store.Add(Build(1, 99));
        }
        catch (InvalidOperationException)
        {
            nonMonotonicRejected = true;
        }

        assert(
            nonMonotonicRejected,
            "Evidence history should reject a generation regression.");

        store.Clear();

        assert(
            store.Count == 0 &&
            store.Latest is null,
            "Evidence history clear should remove all retained manifests.");
    }
}
