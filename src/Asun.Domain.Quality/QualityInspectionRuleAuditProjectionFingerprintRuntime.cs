using System.Security.Cryptography;
using System.Text;

namespace Asun.Domain.Quality;

public static class QualityInspectionRuleAuditProjectionFingerprintRuntime
{
    public static string CreateFingerprint(
        QualityInspectionRuleAuditProjection projection)
    {
        ArgumentNullException.ThrowIfNull(projection);

        var builder=new StringBuilder();

        foreach(var entry in projection.Summary.Entries
                    .OrderBy(item=>item.RuleCode,StringComparer.Ordinal))
        {
            Append(builder,"S");
            Append(builder,entry.RuleCode);
            Append(builder,entry.FindingCount.ToString());
            Append(builder,entry.DistinctEvidenceLinkCount.ToString());

            foreach(var findingId in projection.FindingIndex
                        .FindingsFor(entry.RuleCode)
                        .OrderBy(id=>id.Value,StringComparer.Ordinal))
            {
                Append(builder,"F");
                Append(builder,findingId.Value);
            }

            foreach(var evidenceKey in projection.EvidenceIndex
                        .EvidenceFor(entry.RuleCode)
                        .OrderBy(key=>key.Value,StringComparer.Ordinal))
            {
                Append(builder,"E");
                Append(builder,evidenceKey.Value);
            }
        }

        return Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(builder.ToString())))
            .ToLowerInvariant();
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
