using Asun.Platform.PcbAuditIntegration;
using Asun.Release.Core;

namespace Asun.Platform.PcbAuditReleaseIntegration;

public static class PcbAuditReleaseTransitionProjectionValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        PcbProductionEvidenceEnvelope envelope,
        PcbExecutionAuditWindowProjection auditProjection,
        ReleaseManifest manifest,
        PcbAuditReleaseTransitionProjection projection)
    {
        ArgumentNullException.ThrowIfNull(envelope);
        ArgumentNullException.ThrowIfNull(auditProjection);
        ArgumentNullException.ThrowIfNull(manifest);
        ArgumentNullException.ThrowIfNull(projection);

        var errors=new List<string>();
        if(!ReleaseManifestValidationRuntime.IsValid(manifest))
            errors.Add("Release manifest is invalid.");
        if(auditProjection.EvidenceEnvelopeFingerprint!=envelope.Fingerprint)
            errors.Add("Audit projection must belong to evidence envelope.");
        if(projection.EnvelopeFingerprint!=envelope.Fingerprint)
            errors.Add("Transition envelope fingerprint must match.");
        if(projection.QualityRunId!=auditProjection.QualityRunId)
            errors.Add("Transition Quality run id must match.");
        if(projection.AuditWindowFingerprint!=auditProjection.AuditWindowFingerprint)
            errors.Add("Transition audit window fingerprint must match.");
        if(projection.ReleaseManifestFingerprint!=manifest.Fingerprint)
            errors.Add("Transition manifest fingerprint must match.");
        if(projection.AuditCount!=auditProjection.AuditEnvelopeCount)
            errors.Add("Transition audit count must match.");

        var readiness=ReleaseReadinessRuntime.Evaluate(manifest);
        if(projection.ReleaseReady!=readiness.Ready)
            errors.Add("Transition readiness fact must match.");

        if(projection.Fingerprint.Length!=64 ||
           !projection.Fingerprint.All(character=>
               Uri.IsHexDigit(character) &&
               char.ToLowerInvariant(character)==character))
        {
            errors.Add("Transition fingerprint must be 64 lowercase hexadecimal characters.");
        }

        if(errors.Count>0)
            return errors;

        var expected=PcbAuditReleaseTransitionProjectionRuntime.CreateFingerprint(
            envelope,
            auditProjection,
            manifest,
            readiness.Ready);
        if(expected!=projection.Fingerprint)
            errors.Add("Transition fingerprint does not match canonical content.");

        return errors;
    }

    public static bool IsValid(
        PcbProductionEvidenceEnvelope envelope,
        PcbExecutionAuditWindowProjection auditProjection,
        ReleaseManifest manifest,
        PcbAuditReleaseTransitionProjection projection)=>
        Validate(envelope,auditProjection,manifest,projection).Count==0;
}
