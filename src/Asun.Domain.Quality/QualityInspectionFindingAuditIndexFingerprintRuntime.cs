using System.Security.Cryptography;
using System.Text;

namespace Asun.Domain.Quality;

public static class QualityInspectionFindingAuditIndexFingerprintRuntime
{
    public static string CreateFingerprint(
        QualityInspectionFindingAuditIndex index)
    {
        ArgumentNullException.ThrowIfNull(index);

        var builder=new StringBuilder();

        foreach(var findingId in index.FindingIds)
        {
            var record=index.Find(findingId)
                ?? throw new ArgumentException(
                    "Finding audit index contains an unresolved finding id.",
                    nameof(index));

            Append(builder,record.FindingId.Value);
            Append(builder,record.RuleCode);
            Append(builder,((int)record.Outcome).ToString());
            Append(builder,((int)record.Severity).ToString());
            Append(builder,record.Message);

            foreach(var key in record.EvidenceKeys
                        .OrderBy(item=>item.Value,StringComparer.Ordinal))
            {
                Append(builder,key.Value);
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
