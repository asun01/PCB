namespace Asun.UI.Viewports;

public readonly record struct ViewportReplayBundleVerificationResult(
    bool IsEquivalent,
    bool ManifestMatches,
    bool InputMatches,
    bool EvidenceMatches,
    bool AuditMatches,
    IReadOnlyList<string> Differences)
{
    public static ViewportReplayBundleVerificationResult Equivalent { get; } =
        new(
            true,
            true,
            true,
            true,
            true,
            Array.Empty<string>());
}

public static class ViewportReplayBundleVerificationRuntime
{
    public static ViewportReplayBundleVerificationResult Verify(
        ViewportReplaySessionBundle expected,
        ViewportReplaySessionBundle actual)
    {
        ArgumentNullException.ThrowIfNull(expected);
        ArgumentNullException.ThrowIfNull(actual);

        var expectedErrors =
            ViewportReplaySessionBundleRuntime.Validate(expected);

        if (expectedErrors.Count != 0)
        {
            throw new InvalidOperationException(
                $"Expected replay bundle is invalid: {expectedErrors[0]}");
        }

        var actualErrors =
            ViewportReplaySessionBundleRuntime.Validate(actual);

        if (actualErrors.Count != 0)
        {
            throw new InvalidOperationException(
                $"Actual replay bundle is invalid: {actualErrors[0]}");
        }

        var comparison =
            ViewportReplaySessionBundleRuntime.Compare(
                expected,
                actual);

        if (comparison.IsEquivalent)
            return ViewportReplayBundleVerificationResult.Equivalent;

        var differences = comparison.Differences.ToArray();

        var manifestMatches =
            !differences.Any(
                item => item.StartsWith(
                    "Manifest.",
                    StringComparison.Ordinal) ||
                item.StartsWith(
                    "DerivedManifest.",
                    StringComparison.Ordinal));

        var inputMatches =
            !differences.Any(
                item => item.StartsWith(
                    "Inputs.",
                    StringComparison.Ordinal));

        var evidenceMatches =
            !differences.Any(
                item => item.StartsWith(
                    "Evidence.",
                    StringComparison.Ordinal));

        var auditMatches =
            !differences.Any(
                item => item.StartsWith(
                    "Audit.",
                    StringComparison.Ordinal));

        return new ViewportReplayBundleVerificationResult(
            false,
            manifestMatches,
            inputMatches,
            evidenceMatches,
            auditMatches,
            differences);
    }

    public static bool IsEquivalent(
        ViewportReplayBundleVerificationResult result) =>
        result.IsEquivalent &&
        result.ManifestMatches &&
        result.InputMatches &&
        result.EvidenceMatches &&
        result.AuditMatches;
}
