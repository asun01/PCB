using System.Security.Cryptography;
using System.Text;

namespace Asun.Domain.Quality;

public static class QualityInspectionRunFingerprintRuntime
{
    public static string CreateFingerprint(
        QualityInspectionRun run)
    {
        ArgumentNullException.ThrowIfNull(run);

        if(!QualityInspectionRunValidationRuntime.IsValid(run))
            throw new ArgumentException(
                "Inspection run is invalid.",
                nameof(run));

        var builder=new StringBuilder();
        builder.Append(run.RunId).Append('|');

        foreach(var result in run.Results)
        {
            var summary=QualityInspectionSummaryRuntime.Create(result);
            builder.Append(summary.ResultId).Append('|')
                .Append(summary.SnapshotId).Append('|')
                .Append(summary.Sequence).Append('|')
                .Append(summary.ContentFingerprint).Append('|');
        }

        return Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(builder.ToString())))
            .ToLowerInvariant();
    }
}
