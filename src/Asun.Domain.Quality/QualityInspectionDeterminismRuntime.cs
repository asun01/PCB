using System.Security.Cryptography;
using System.Text;

namespace Asun.Domain.Quality;

public static class QualityInspectionDeterminismRuntime
{
    public static string CreateContentFingerprint(
        QualityInspectionSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        if (!QualityInspectionSnapshotValidationRuntime.IsValid(snapshot))
            throw new ArgumentException(
                "Inspection snapshot is invalid.",
                nameof(snapshot));

        var builder = new StringBuilder();

        foreach (var finding in snapshot.Findings.Findings
                     .OrderBy(item => item.Id.Value, StringComparer.Ordinal))
        {
            Append(builder, finding.Id.Value);
            Append(builder, finding.RuleCode);
            Append(builder, ((int)finding.Outcome).ToString());
            Append(builder, ((int)finding.Severity).ToString());
            Append(builder, finding.Message);
        }

        foreach (var link in snapshot.Evidence.Links
                     .OrderBy(item => item.FindingId.Value, StringComparer.Ordinal)
                     .ThenBy(item => item.EvidenceKey.Value, StringComparer.Ordinal))
        {
            Append(builder, link.FindingId.Value);
            Append(builder, link.EvidenceKey.Value);
        }

        var hash = SHA256.HashData(
            Encoding.UTF8.GetBytes(builder.ToString()));

        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    public static bool AreContentEquivalent(
        QualityInspectionSnapshot left,
        QualityInspectionSnapshot right) =>
        string.Equals(
            CreateContentFingerprint(left),
            CreateContentFingerprint(right),
            StringComparison.Ordinal);

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
