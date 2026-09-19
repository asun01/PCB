using System.Security.Cryptography;
using System.Text;

namespace Asun.Domain.Quality;

public static class QualityInspectionAuditProjectionFingerprintRuntime
{
    public static string CreateFingerprint(
        QualityInspectionAuditProjection projection)
    {
        ArgumentNullException.ThrowIfNull(projection);

        if(projection.Record.ResultId==Guid.Empty ||
           projection.Record.SnapshotId==Guid.Empty)
        {
            throw new ArgumentException(
                "Audit projection identity is invalid.",
                nameof(projection));
        }

        var builder=new StringBuilder();
        Append(builder,projection.Record.ResultId.ToString("D"));
        Append(builder,projection.Record.SnapshotId.ToString("D"));
        Append(builder,projection.Record.Sequence);
        Append(builder,projection.Record.ContentFingerprint);
        Append(builder,projection.Summary.ContentFingerprint);

        Append(
            builder,
            QualityInspectionRuleAuditProjectionFingerprintRuntime
                .CreateFingerprint(projection.RuleAudit));

        Append(
            builder,
            QualityInspectionFindingAuditIndexFingerprintRuntime
                .CreateFingerprint(projection.FindingAudit.Index));

        return Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(builder.ToString())))
            .ToLowerInvariant();
    }

    private static void Append(StringBuilder builder,string value)
    {
        builder.Append(value.Length).Append(':').Append(value).Append('|');
    }

    private static void Append(StringBuilder builder,long value) =>
        Append(builder,value.ToString());
}
