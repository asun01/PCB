using System.Security.Cryptography;
using System.Text;
using Asun.Domain.Quality;
using Asun.Platform.PcbEvidenceEnvelopeIntegration;

namespace Asun.Platform.PcbAuditIntegration;

public static class PcbExecutionAuditWindowProjectionRuntime
{
    public static PcbExecutionAuditWindowProjection Create(
        PcbProductionEvidenceEnvelope envelope,
        QualityInspectionRun qualityRun)
    {
        ArgumentNullException.ThrowIfNull(envelope);
        ArgumentNullException.ThrowIfNull(qualityRun);

        if(!QualityInspectionRunValidationRuntime.IsValid(qualityRun))
            throw new ArgumentException("Quality inspection run is invalid.",nameof(qualityRun));

        var window=QualityInspectionAuditWindowRuntime.Create(qualityRun.Results);
        var auditWindowFingerprint=CreateAuditWindowFingerprint(window);

        var fingerprint=CreateFingerprint(
            envelope.Fingerprint,
            qualityRun.RunId,
            auditWindowFingerprint);

        return new PcbExecutionAuditWindowProjection(
            envelope.Fingerprint,
            qualityRun.RunId,
            window.Count,
            auditWindowFingerprint,
            fingerprint);
    }

    internal static string CreateAuditWindowFingerprint(
        QualityInspectionAuditWindow window)
    {
        var canonical=new StringBuilder();
        foreach(var envelope in window.Envelopes)
        {
            canonical.Append(envelope.ResultId).Append('|')
                .Append(envelope.SnapshotId).Append('|')
                .Append(envelope.Sequence).Append('|')
                .Append(envelope.Fingerprint).Append('|');
        }

        return Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(canonical.ToString())))
            .ToLowerInvariant();
    }

    internal static string CreateFingerprint(
        string envelopeFingerprint,
        Guid qualityRunId,
        string auditWindowFingerprint)
    {
        var canonical=string.Join("|",envelopeFingerprint,qualityRunId,auditWindowFingerprint);
        return Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(canonical)))
            .ToLowerInvariant();
    }
}
