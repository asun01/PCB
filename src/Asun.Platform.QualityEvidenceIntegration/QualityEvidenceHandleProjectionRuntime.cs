using System.Security.Cryptography;
using System.Text;
using Asun.Domain.Quality;

namespace Asun.Platform.QualityEvidenceIntegration;

public static class QualityEvidenceHandleProjectionRuntime
{
    public static QualityEvidenceHandleProjection Create(
        QualityInspectionRun run,
        IReadOnlyList<QualityEvidenceHandleBinding> bindings)
    {
        ArgumentNullException.ThrowIfNull(run);
        ArgumentNullException.ThrowIfNull(bindings);

        var normalized=QualityEvidenceHandleBindingRuntime.Create(run,bindings);
        var fingerprint=CreateFingerprint(run.RunId,normalized);

        return new QualityEvidenceHandleProjection(
            run.RunId,
            normalized,
            fingerprint);
    }

    internal static string CreateFingerprint(
        Guid runId,
        IReadOnlyList<QualityEvidenceHandleBinding> bindings)
    {
        var builder=new StringBuilder();
        builder.Append(runId).Append('|');

        foreach(var binding in bindings.OrderBy(item=>item.FindingId.Value,StringComparer.Ordinal).ThenBy(item=>item.EvidenceKey.Value,StringComparer.Ordinal))
        {
            builder.Append(binding.FindingId.Value.Length).Append(':').Append(binding.FindingId.Value).Append('|')
                .Append(binding.EvidenceKey.Value.Length).Append(':').Append(binding.EvidenceKey.Value).Append('|')
                .Append(binding.EvidenceHandle.Value.Length).Append(':').Append(binding.EvidenceHandle.Value).Append('|');
        }

        return Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(builder.ToString())))
            .ToLowerInvariant();
    }
}
