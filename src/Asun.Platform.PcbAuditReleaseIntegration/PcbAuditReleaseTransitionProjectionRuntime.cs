using System.Security.Cryptography;
using System.Text;
using Asun.Platform.PcbAuditIntegration;
using Asun.Release.Core;

namespace Asun.Platform.PcbAuditReleaseIntegration;

public static class PcbAuditReleaseTransitionProjectionRuntime
{
    public static PcbAuditReleaseTransitionProjection Create(
        PcbProductionEvidenceEnvelope envelope,
        PcbExecutionAuditWindowProjection auditProjection,
        ReleaseManifest manifest)
    {
        ArgumentNullException.ThrowIfNull(envelope);
        ArgumentNullException.ThrowIfNull(auditProjection);
        ArgumentNullException.ThrowIfNull(manifest);

        if(!ReleaseManifestValidationRuntime.IsValid(manifest))
            throw new ArgumentException("Release manifest is invalid.",nameof(manifest));
        if(auditProjection.EvidenceEnvelopeFingerprint!=envelope.Fingerprint)
            throw new ArgumentException("Audit projection must belong to the evidence envelope.",nameof(auditProjection));

        var readiness=ReleaseReadinessRuntime.Evaluate(manifest);
        var fingerprint=CreateFingerprint(envelope,auditProjection,manifest,readiness.Ready);

        return new PcbAuditReleaseTransitionProjection(
            envelope.Fingerprint,
            auditProjection.QualityRunId,
            auditProjection.AuditWindowFingerprint,
            manifest.Fingerprint,
            auditProjection.AuditEnvelopeCount,
            readiness.Ready,
            fingerprint);
    }

    internal static string CreateFingerprint(
        PcbProductionEvidenceEnvelope envelope,
        PcbExecutionAuditWindowProjection auditProjection,
        ReleaseManifest manifest,
        bool releaseReady)
    {
        var canonical=string.Join(
            "|",
            envelope.Fingerprint,
            auditProjection.QualityRunId,
            auditProjection.AuditWindowFingerprint,
            manifest.Fingerprint,
            auditProjection.AuditEnvelopeCount,
            releaseReady);
        return Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(canonical)))
            .ToLowerInvariant();
    }
}
