namespace Asun.UI.Viewports;

public readonly record struct ViewportReplaySessionBundleWindow(
    int InputCount,
    int EvidenceCount,
    int AuditCount,
    long FirstInputSequence,
    long LastInputSequence,
    long FirstEvidenceGeneration,
    long LastEvidenceGeneration,
    long FirstAuditSequence,
    long LastAuditSequence)
{
    public static ViewportReplaySessionBundleWindow Empty { get; } =
        new(0, 0, 0, 0, 0, 0, 0, 0);
}

public static class ViewportReplaySessionBundleWindowRuntime
{
    public static ViewportReplaySessionBundleWindow Describe(
        ViewportReplaySessionBundle bundle)
    {
        return new(
            bundle.Inputs.Length,
            bundle.Evidence.Length,
            bundle.Audit.Length,
            bundle.Inputs.Length == 0 ? 0 : bundle.Inputs[0].Sequence,
            bundle.Inputs.Length == 0 ? 0 : bundle.Inputs[^1].Sequence,
            bundle.Evidence.Length == 0 ? 0 : bundle.Evidence[0].Generation,
            bundle.Evidence.Length == 0 ? 0 : bundle.Evidence[^1].Generation,
            bundle.Audit.Length == 0 ? 0 : bundle.Audit[0].Sequence,
            bundle.Audit.Length == 0 ? 0 : bundle.Audit[^1].Sequence);
    }

    public static ViewportReplaySessionBundle Tail(
        ViewportReplaySessionBundle bundle,
        int maxInputs,
        int maxEvidence,
        int maxAudit)
    {
        if (maxInputs < 0)
            throw new ArgumentOutOfRangeException(nameof(maxInputs));

        if (maxEvidence < 0)
            throw new ArgumentOutOfRangeException(nameof(maxEvidence));

        if (maxAudit < 0)
            throw new ArgumentOutOfRangeException(nameof(maxAudit));

        var inputs = TakeLast(
            bundle.Inputs,
            maxInputs);

        var audits = TakeLast(
            bundle.Audit,
            maxAudit);

        var referencedEvidenceKeys = audits
            .Select(item => item.EvidenceKey)
            .Where(key => !string.IsNullOrEmpty(key))
            .ToHashSet(StringComparer.Ordinal);

        var required = referencedEvidenceKeys.Count;

        if (required > maxEvidence)
        {
            throw new InvalidOperationException(
                "maxEvidence is too small to retain all evidence referenced by the selected audit window.");
        }

        var recentEvidence = bundle.Evidence
            .TakeLast(maxEvidence)
            .ToList();

        var requiredEvidence = bundle.Evidence
            .Where(item => referencedEvidenceKeys.Contains(item.StableKey))
            .ToList();

        var evidence = requiredEvidence
            .Concat(
                recentEvidence.Where(item =>
                    !referencedEvidenceKeys.Contains(item.StableKey)))
            .OrderBy(item => item.Generation)
            .ThenBy(item => item.SubmissionSequence)
            .ToList();

        if (evidence.Count > maxEvidence)
        {
            evidence = evidence
                .Where(item => referencedEvidenceKeys.Contains(item.StableKey))
                .Concat(
                    evidence
                        .Where(item => !referencedEvidenceKeys.Contains(item.StableKey))
                        .Take(maxEvidence - required))
                .OrderBy(item => item.Generation)
                .ThenBy(item => item.SubmissionSequence)
                .ToList();
        }

        var manifest = ViewportReplaySessionBundleRuntime.CreateManifest(
            bundle.Manifest.SessionId,
            bundle.Manifest.CreatedAtUtc,
            inputs,
            evidence,
            audits);

        return ViewportReplaySessionBundleRuntime.Capture(
            manifest,
            inputs,
            evidence,
            audits);
    }

    public static bool IsTailOf(
        ViewportReplaySessionBundle source,
        ViewportReplaySessionBundle candidate)
    {
        if (!string.Equals(
                source.Manifest.SessionId,
                candidate.Manifest.SessionId,
                StringComparison.Ordinal))
        {
            return false;
        }

        if (!candidate.Inputs.All(
                item => source.Inputs.Contains(item)))
        {
            return false;
        }

        if (!candidate.Evidence.All(
                item => source.Evidence.Contains(item)))
        {
            return false;
        }

        return candidate.Audit.All(
            item => source.Audit.Contains(item));
    }

    private static T[] TakeLast<T>(
        IReadOnlyList<T> values,
        int count)
    {
        if (count == 0 || values.Count == 0)
            return Array.Empty<T>();

        var take = Math.Min(count, values.Count);

        return values
            .Skip(values.Count - take)
            .ToArray();
    }
}
