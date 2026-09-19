using System.Security.Cryptography;
using System.Text;

namespace Asun.Domain.Quality;

public static class QualityInspectionReplayBundleFingerprintRuntime
{
    public static string CreateFingerprint(
        QualityInspectionReplayBundle bundle)
    {
        ArgumentNullException.ThrowIfNull(bundle);

        if (!QualityInspectionReplayBundleValidationRuntime.IsValid(bundle))
            throw new ArgumentException(
                "Replay bundle is invalid.",
                nameof(bundle));

        var builder = new StringBuilder();

        Append(builder, bundle.Previous is null ? "none" : "present");

        if (bundle.Previous is not null)
            AppendProjection(builder, bundle.Previous);

        AppendProjection(builder, bundle.Current);

        foreach (var id in bundle.Diff.AddedFindingIds
                     .OrderBy(item => item.Value, StringComparer.Ordinal))
        {
            Append(builder, "AF");
            Append(builder, id.Value);
        }

        foreach (var id in bundle.Diff.RemovedFindingIds
                     .OrderBy(item => item.Value, StringComparer.Ordinal))
        {
            Append(builder, "RF");
            Append(builder, id.Value);
        }

        foreach (var link in bundle.Diff.AddedEvidenceLinks
                     .OrderBy(item => item.FindingId.Value, StringComparer.Ordinal)
                     .ThenBy(item => item.EvidenceKey.Value, StringComparer.Ordinal))
        {
            Append(builder, "AE");
            Append(builder, link.FindingId.Value);
            Append(builder, link.EvidenceKey.Value);
        }

        foreach (var link in bundle.Diff.RemovedEvidenceLinks
                     .OrderBy(item => item.FindingId.Value, StringComparer.Ordinal)
                     .ThenBy(item => item.EvidenceKey.Value, StringComparer.Ordinal))
        {
            Append(builder, "RE");
            Append(builder, link.FindingId.Value);
            Append(builder, link.EvidenceKey.Value);
        }

        Append(builder, bundle.Diff.ContentFingerprintChanged ? "1" : "0");

        return Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(builder.ToString())))
            .ToLowerInvariant();
    }

    public static bool AreEquivalent(
        QualityInspectionReplayBundle left,
        QualityInspectionReplayBundle right) =>
        string.Equals(
            CreateFingerprint(left),
            CreateFingerprint(right),
            StringComparison.Ordinal);

    private static void AppendProjection(
        StringBuilder builder,
        QualityInspectionReplayProjection projection)
    {
        Append(builder, projection.ResultId.ToString("D"));
        Append(builder, projection.SnapshotId.ToString("D"));
        Append(builder, projection.Sequence.ToString());

        foreach (var id in projection.FindingIds
                     .OrderBy(item => item.Value, StringComparer.Ordinal))
        {
            Append(builder, "F");
            Append(builder, id.Value);
        }

        foreach (var link in projection.EvidenceManifest.Links
                     .OrderBy(item => item.FindingId.Value, StringComparer.Ordinal)
                     .ThenBy(item => item.EvidenceKey.Value, StringComparer.Ordinal))
        {
            Append(builder, "E");
            Append(builder, link.FindingId.Value);
            Append(builder, link.EvidenceKey.Value);
        }

        Append(builder, projection.ContentFingerprint);
    }

    private static void Append(
        StringBuilder builder,
        string value)
    {
        builder
            .Append(value.Length)
            .Append(':')
            .Append(value)
            .Append('|');
    }
}
