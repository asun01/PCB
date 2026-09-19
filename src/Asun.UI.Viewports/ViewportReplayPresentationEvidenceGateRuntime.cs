namespace Asun.UI.Viewports;

public readonly record struct ViewportReplayPresentationEvidenceGateResult(
    bool IsEquivalent,
    bool EvidenceMatches,
    bool AuditMatches,
    bool OrderingMatches,
    IReadOnlyList<string> Differences)
{
    public static ViewportReplayPresentationEvidenceGateResult Equivalent { get; } =
        new(
            true,
            true,
            true,
            true,
            Array.Empty<string>());
}

public static class ViewportReplayPresentationEvidenceGateRuntime
{
    public static ViewportReplayPresentationEvidenceGateResult Verify(
        IReadOnlyList<ViewportRenderEvidenceManifest> expectedEvidence,
        IReadOnlyList<ViewportRenderEvidenceManifest> actualEvidence,
        IReadOnlyList<ViewportPresentationAuditEvent> expectedAudit,
        IReadOnlyList<ViewportPresentationAuditEvent> actualAudit)
    {
        ArgumentNullException.ThrowIfNull(expectedEvidence);
        ArgumentNullException.ThrowIfNull(actualEvidence);
        ArgumentNullException.ThrowIfNull(expectedAudit);
        ArgumentNullException.ThrowIfNull(actualAudit);

        var differences = new List<string>();
        var evidenceMatches = true;
        var auditMatches = true;
        var orderingMatches = true;

        for (var index = 0; index < expectedEvidence.Count; index++)
        {
            var errors =
                ViewportRenderDiagnosticsRuntime.ValidateManifest(
                    expectedEvidence[index]);

            foreach (var error in errors)
            {
                evidenceMatches = false;
                differences.Add(
                    $"ExpectedEvidence[{index}].{error}");
            }
        }

        for (var index = 0; index < actualEvidence.Count; index++)
        {
            var errors =
                ViewportRenderDiagnosticsRuntime.ValidateManifest(
                    actualEvidence[index]);

            foreach (var error in errors)
            {
                evidenceMatches = false;
                differences.Add(
                    $"ActualEvidence[{index}].{error}");
            }
        }

        if (expectedEvidence.Count != actualEvidence.Count)
        {
            evidenceMatches = false;
            differences.Add(
                $"Evidence.Count: expected '{expectedEvidence.Count}', actual '{actualEvidence.Count}'.");
        }

        var evidenceCount =
            Math.Min(expectedEvidence.Count, actualEvidence.Count);

        for (var i = 0; i < evidenceCount; i++)
        {
            var comparison =
                ViewportRenderEvidenceComparatorRuntime.Compare(
                    expectedEvidence[i],
                    actualEvidence[i]);

            if (!comparison.IsEquivalent)
            {
                evidenceMatches = false;

                foreach (var difference in comparison.Differences)
                {
                    differences.Add(
                        $"Evidence[{i}].{difference}");
                }
            }
        }

        if (expectedAudit.Count != actualAudit.Count)
        {
            auditMatches = false;
            differences.Add(
                $"Audit.Count: expected '{expectedAudit.Count}', actual '{actualAudit.Count}'.");
        }

        var auditCount =
            Math.Min(expectedAudit.Count, actualAudit.Count);

        for (var i = 0; i < auditCount; i++)
        {
            var expected = expectedAudit[i];
            var actual = actualAudit[i];

            CompareAuditValue(
                differences,
                ref auditMatches,
                i,
                nameof(ViewportPresentationAuditEvent.Sequence),
                expected.Sequence,
                actual.Sequence);
            CompareAuditValue(
                differences,
                ref auditMatches,
                i,
                nameof(ViewportPresentationAuditEvent.Stage),
                expected.Stage,
                actual.Stage);
            CompareAuditValue(
                differences,
                ref auditMatches,
                i,
                nameof(ViewportPresentationAuditEvent.Generation),
                expected.Generation,
                actual.Generation);
            CompareAuditValue(
                differences,
                ref auditMatches,
                i,
                nameof(ViewportPresentationAuditEvent.SubmissionSequence),
                expected.SubmissionSequence,
                actual.SubmissionSequence);
            CompareAuditValue(
                differences,
                ref auditMatches,
                i,
                nameof(ViewportPresentationAuditEvent.DeliveryStatus),
                expected.DeliveryStatus,
                actual.DeliveryStatus);
            CompareAuditValue(
                differences,
                ref auditMatches,
                i,
                nameof(ViewportPresentationAuditEvent.RenderedUnits),
                expected.RenderedUnits,
                actual.RenderedUnits);
            CompareAuditValue(
                differences,
                ref auditMatches,
                i,
                nameof(ViewportPresentationAuditEvent.DeferredUnits),
                expected.DeferredUnits,
                actual.DeferredUnits);
            CompareAuditValue(
                differences,
                ref auditMatches,
                i,
                nameof(ViewportPresentationAuditEvent.EvidenceKey),
                expected.EvidenceKey,
                actual.EvidenceKey);
        }

        orderingMatches =
            IsEvidenceOrdered(expectedEvidence) &&
            IsEvidenceOrdered(actualEvidence) &&
            IsAuditOrdered(expectedAudit) &&
            IsAuditOrdered(actualAudit);

        if (!orderingMatches)
            differences.Add(
                "Evidence or audit ordering is not monotonic.");

        return differences.Count == 0
            ? ViewportReplayPresentationEvidenceGateResult.Equivalent
            : new ViewportReplayPresentationEvidenceGateResult(
                false,
                evidenceMatches,
                auditMatches,
                orderingMatches,
                differences);
    }

    public static bool IsEquivalent(
        ViewportReplayPresentationEvidenceGateResult result) =>
        result.IsEquivalent &&
        result.EvidenceMatches &&
        result.AuditMatches &&
        result.OrderingMatches;

    private static bool IsEvidenceOrdered(
        IReadOnlyList<ViewportRenderEvidenceManifest> evidence)
    {
        long previousGeneration = -1;
        long previousSequence = -1;

        foreach (var item in evidence)
        {
            if (item.Generation < previousGeneration ||
                (item.Generation == previousGeneration &&
                 item.SubmissionSequence < previousSequence))
            {
                return false;
            }

            previousGeneration = item.Generation;
            previousSequence = item.SubmissionSequence;
        }

        return true;
    }

    private static bool IsAuditOrdered(
        IReadOnlyList<ViewportPresentationAuditEvent> audit)
    {
        long previous = 0;

        foreach (var item in audit)
        {
            if (item.Sequence <= previous)
                return false;

            previous = item.Sequence;
        }

        return true;
    }

    private static void CompareAuditValue<T>(
        ICollection<string> differences,
        ref bool matches,
        int index,
        string name,
        T expected,
        T actual)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
        {
            matches = false;
            differences.Add(
                $"Audit[{index}].{name}: expected '{expected}', actual '{actual}'.");
        }
    }
}
