using System.Text;
using System.Text.Json;

namespace Asun.UI.Viewports;

public readonly record struct ViewportReplaySessionBundleComparison(
    bool IsEquivalent,
    IReadOnlyList<string> Differences)
{
    public static ViewportReplaySessionBundleComparison Equivalent { get; } =
        new(true, Array.Empty<string>());
}

public static class ViewportReplaySessionBundleRuntime
{
    public static string ToJson(
        ViewportReplaySessionBundle bundle)
    {
        var errors = Validate(bundle);

        if (errors.Count != 0)
        {
            throw new InvalidOperationException(
                $"Cannot serialize an invalid replay bundle: {errors[0]}");
        }

        return JsonSerializer.Serialize(
            bundle,
            new JsonSerializerOptions
            {
                WriteIndented = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
    }

    public static ViewportReplaySessionBundle FromJson(
        string json)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);

        var bundle = JsonSerializer.Deserialize<ViewportReplaySessionBundle>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        if (bundle is null)
            throw new InvalidOperationException(
                "Replay bundle JSON did not contain a bundle.");

        var errors = Validate(bundle);

        if (errors.Count != 0)
        {
            throw new InvalidOperationException(
                $"Replay bundle JSON failed validation: {errors[0]}");
        }

        return bundle;
    }

    public static ViewportReplaySessionBundle Capture(
        ViewportReplaySessionManifest manifest,
        IReadOnlyList<ViewportInputEvent> inputs,
        IReadOnlyList<ViewportRenderEvidenceManifest> evidence,
        IReadOnlyList<ViewportPresentationAuditEvent> audit)
    {
        ArgumentNullException.ThrowIfNull(inputs);
        ArgumentNullException.ThrowIfNull(evidence);
        ArgumentNullException.ThrowIfNull(audit);

        return new ViewportReplaySessionBundle(
            manifest,
            inputs.ToArray(),
            evidence.ToArray(),
            audit.ToArray());
    }

    public static IReadOnlyList<string> Validate(
        ViewportReplaySessionBundle bundle)
    {
        var errors = new List<string>();
        var manifest = bundle.Manifest;

        if (manifest.InputEventCount != bundle.Inputs.Length)
            errors.Add("Manifest input count does not match bundle input count.");

        if (manifest.EvidenceManifestCount != bundle.Evidence.Length)
            errors.Add("Manifest evidence count does not match bundle evidence count.");

        if (manifest.AuditEventCount != bundle.Audit.Length)
            errors.Add("Manifest audit count does not match bundle audit count.");

        long previousInputSequence = 0;

        foreach (var input in bundle.Inputs)
        {
            if (input.Sequence <= previousInputSequence)
                errors.Add("Bundle input sequence must increase strictly.");

            previousInputSequence = input.Sequence;
        }

        long previousEvidenceGeneration = -1;
        long previousEvidenceSequence = -1;

        foreach (var evidence in bundle.Evidence)
        {
            if (evidence.Generation < previousEvidenceGeneration ||
                (evidence.Generation == previousEvidenceGeneration &&
                 evidence.SubmissionSequence < previousEvidenceSequence))
            {
                errors.Add("Bundle evidence sequence must remain monotonic.");
            }

            errors.AddRange(
                ViewportRenderDiagnosticsRuntime.ValidateManifest(
                    evidence));

            previousEvidenceGeneration = evidence.Generation;
            previousEvidenceSequence = evidence.SubmissionSequence;
        }

        long previousAuditSequence = 0;
        var evidenceKeys = bundle.Evidence
            .Select(item => item.StableKey)
            .ToHashSet(StringComparer.Ordinal);

        foreach (var audit in bundle.Audit)
        {
            if (audit.Sequence <= previousAuditSequence)
                errors.Add("Bundle audit sequence must increase strictly.");

            if (audit.Generation < 0 ||
                audit.SubmissionSequence < 0 ||
                audit.RenderedUnits < 0 ||
                audit.DeferredUnits < 0)
            {
                errors.Add("Bundle audit counters must be non-negative.");
            }

            if (!string.IsNullOrEmpty(audit.EvidenceKey) &&
                !evidenceKeys.Contains(audit.EvidenceKey))
            {
                errors.Add(
                    "Bundle audit evidence key does not reference retained evidence.");
            }

            previousAuditSequence = audit.Sequence;
        }

        var recomputed = CreateManifest(
            manifest.SessionId,
            manifest.CreatedAtUtc,
            bundle.Inputs,
            bundle.Evidence,
            bundle.Audit);

        if (recomputed.InputHash != manifest.InputHash)
            errors.Add("Bundle input hash does not match its manifest.");

        if (recomputed.EvidenceHash != manifest.EvidenceHash)
            errors.Add("Bundle evidence hash does not match its manifest.");

        if (recomputed.AuditHash != manifest.AuditHash)
            errors.Add("Bundle audit hash does not match its manifest.");

        if (recomputed.SessionHash != manifest.SessionHash)
            errors.Add("Bundle session hash does not match its manifest.");

        return errors;
    }

    public static ViewportReplaySessionManifest CreateManifest(
        string sessionId,
        DateTimeOffset createdAtUtc,
        IReadOnlyList<ViewportInputEvent> inputs,
        IReadOnlyList<ViewportRenderEvidenceManifest> evidence,
        IReadOnlyList<ViewportPresentationAuditEvent> audit)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sessionId);
        ArgumentNullException.ThrowIfNull(inputs);
        ArgumentNullException.ThrowIfNull(evidence);
        ArgumentNullException.ThrowIfNull(audit);

        var inputHash = Hash(
            string.Join(
                "\n",
                inputs.Select(item =>
                    $"{item.Sequence}|{item.Kind}|{item.Position.X:R}|{item.Position.Y:R}|{item.WheelDelta}|{item.Button}")));

        var evidenceHash = Hash(
            string.Join(
                "\n",
                evidence.Select(item =>
                    $"{item.Generation}|{item.SubmissionSequence}|{item.FrameHash}|{item.BatchHash}|{item.CommandHash}|{item.ReplayHash}")));

        var auditHash = Hash(
            string.Join(
                "\n",
                audit.Select(item =>
                    $"{item.Sequence}|{item.Stage}|{item.Generation}|{item.SubmissionSequence}|{item.DeliveryStatus}|{item.RenderedUnits}|{item.DeferredUnits}|{item.EvidenceKey}")));

        var sessionHash = Hash(
            $"{sessionId}|{createdAtUtc:O}|{inputHash}|{evidenceHash}|{auditHash}");

        return new ViewportReplaySessionManifest(
            sessionId,
            createdAtUtc,
            inputs.Count,
            evidence.Count,
            audit.Count,
            inputHash,
            evidenceHash,
            auditHash,
            sessionHash);
    }

    public static ViewportReplaySessionBundleComparison Compare(
        ViewportReplaySessionBundle expected,
        ViewportReplaySessionBundle actual)
    {
        var differences = new List<string>();

        CompareValue(
            differences,
            "Manifest.SessionId",
            expected.Manifest.SessionId,
            actual.Manifest.SessionId);

        CompareValue(
            differences,
            "Manifest.CreatedAtUtc",
            expected.Manifest.CreatedAtUtc,
            actual.Manifest.CreatedAtUtc);
        CompareValue(
            differences,
            "Manifest.InputEventCount",
            expected.Manifest.InputEventCount,
            actual.Manifest.InputEventCount);
        CompareValue(
            differences,
            "Manifest.EvidenceManifestCount",
            expected.Manifest.EvidenceManifestCount,
            actual.Manifest.EvidenceManifestCount);
        CompareValue(
            differences,
            "Manifest.AuditEventCount",
            expected.Manifest.AuditEventCount,
            actual.Manifest.AuditEventCount);
        CompareValue(
            differences,
            "Manifest.InputHash",
            expected.Manifest.InputHash,
            actual.Manifest.InputHash);
        CompareValue(
            differences,
            "Manifest.EvidenceHash",
            expected.Manifest.EvidenceHash,
            actual.Manifest.EvidenceHash);
        CompareValue(
            differences,
            "Manifest.AuditHash",
            expected.Manifest.AuditHash,
            actual.Manifest.AuditHash);
        CompareValue(
            differences,
            "Manifest.SessionHash",
            expected.Manifest.SessionHash,
            actual.Manifest.SessionHash);

        CompareInputs(differences, expected.Inputs, actual.Inputs);
        CompareEvidence(differences, expected.Evidence, actual.Evidence);
        CompareAudit(differences, expected.Audit, actual.Audit);

        var expectedManifest = CreateManifest(
            expected.Manifest.SessionId,
            expected.Manifest.CreatedAtUtc,
            expected.Inputs,
            expected.Evidence,
            expected.Audit);

        var actualManifest = CreateManifest(
            actual.Manifest.SessionId,
            actual.Manifest.CreatedAtUtc,
            actual.Inputs,
            actual.Evidence,
            actual.Audit);

        CompareValue(
            differences,
            "DerivedManifest.InputHash",
            expectedManifest.InputHash,
            actualManifest.InputHash);

        CompareValue(
            differences,
            "DerivedManifest.EvidenceHash",
            expectedManifest.EvidenceHash,
            actualManifest.EvidenceHash);

        CompareValue(
            differences,
            "DerivedManifest.AuditHash",
            expectedManifest.AuditHash,
            actualManifest.AuditHash);

        CompareValue(
            differences,
            "DerivedManifest.SessionHash",
            expectedManifest.SessionHash,
            actualManifest.SessionHash);

        return differences.Count == 0
            ? ViewportReplaySessionBundleComparison.Equivalent
            : new ViewportReplaySessionBundleComparison(false, differences);
    }

    public static bool AreEquivalent(
        ViewportReplaySessionBundle expected,
        ViewportReplaySessionBundle actual) =>
        Compare(expected, actual).IsEquivalent;

    private static void CompareInputs(
        ICollection<string> differences,
        IReadOnlyList<ViewportInputEvent> expected,
        IReadOnlyList<ViewportInputEvent> actual)
    {
        CompareValue(
            differences,
            "Inputs.Count",
            expected.Count,
            actual.Count);

        var count = Math.Min(expected.Count, actual.Count);

        for (var index = 0; index < count; index++)
        {
            var left = expected[index];
            var right = actual[index];

            CompareValue(
                differences,
                $"Inputs[{index}].Sequence",
                left.Sequence,
                right.Sequence);
            CompareValue(
                differences,
                $"Inputs[{index}].Kind",
                left.Kind,
                right.Kind);
            CompareValue(
                differences,
                $"Inputs[{index}].Position",
                left.Position,
                right.Position);
            CompareValue(
                differences,
                $"Inputs[{index}].WheelDelta",
                left.WheelDelta,
                right.WheelDelta);
            CompareValue(
                differences,
                $"Inputs[{index}].Button",
                left.Button,
                right.Button);
        }
    }

    private static void CompareEvidence(
        ICollection<string> differences,
        IReadOnlyList<ViewportRenderEvidenceManifest> expected,
        IReadOnlyList<ViewportRenderEvidenceManifest> actual)
    {
        CompareValue(
            differences,
            "Evidence.Count",
            expected.Count,
            actual.Count);

        var count = Math.Min(expected.Count, actual.Count);

        for (var index = 0; index < count; index++)
        {
            var left = expected[index];
            var right = actual[index];

            CompareValue(
                differences,
                $"Evidence[{index}].Generation",
                left.Generation,
                right.Generation);
            CompareValue(
                differences,
                $"Evidence[{index}].SubmissionSequence",
                left.SubmissionSequence,
                right.SubmissionSequence);
            CompareValue(
                differences,
                $"Evidence[{index}].DirtyFlags",
                left.DirtyFlags,
                right.DirtyFlags);
            CompareValue(
                differences,
                $"Evidence[{index}].BatchHash",
                left.BatchHash,
                right.BatchHash);
            CompareValue(
                differences,
                $"Evidence[{index}].CommandHash",
                left.CommandHash,
                right.CommandHash);
            CompareValue(
                differences,
                $"Evidence[{index}].FrameHash",
                left.FrameHash,
                right.FrameHash);
            CompareValue(
                differences,
                $"Evidence[{index}].ReplayHash",
                left.ReplayHash,
                right.ReplayHash);
        }
    }

    private static void CompareAudit(
        ICollection<string> differences,
        IReadOnlyList<ViewportPresentationAuditEvent> expected,
        IReadOnlyList<ViewportPresentationAuditEvent> actual)
    {
        CompareValue(
            differences,
            "Audit.Count",
            expected.Count,
            actual.Count);

        var count = Math.Min(expected.Count, actual.Count);

        for (var index = 0; index < count; index++)
        {
            var left = expected[index];
            var right = actual[index];

            CompareValue(
                differences,
                $"Audit[{index}].Sequence",
                left.Sequence,
                right.Sequence);
            CompareValue(
                differences,
                $"Audit[{index}].Stage",
                left.Stage,
                right.Stage);
            CompareValue(
                differences,
                $"Audit[{index}].Generation",
                left.Generation,
                right.Generation);
            CompareValue(
                differences,
                $"Audit[{index}].SubmissionSequence",
                left.SubmissionSequence,
                right.SubmissionSequence);
            CompareValue(
                differences,
                $"Audit[{index}].DeliveryStatus",
                left.DeliveryStatus,
                right.DeliveryStatus);
            CompareValue(
                differences,
                $"Audit[{index}].RenderedUnits",
                left.RenderedUnits,
                right.RenderedUnits);
            CompareValue(
                differences,
                $"Audit[{index}].DeferredUnits",
                left.DeferredUnits,
                right.DeferredUnits);
            CompareValue(
                differences,
                $"Audit[{index}].EvidenceKey",
                left.EvidenceKey,
                right.EvidenceKey);
        }
    }

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

    private static string Hash(string value) =>
        ViewportRenderEvidenceRuntime.ComputeTextHash(
            value);
}
