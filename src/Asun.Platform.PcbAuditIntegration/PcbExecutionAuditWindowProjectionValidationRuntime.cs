using Asun.Domain.Quality;
using Asun.Platform.PcbEvidenceEnvelopeIntegration;

namespace Asun.Platform.PcbAuditIntegration;

public static class PcbExecutionAuditWindowProjectionValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        PcbProductionEvidenceEnvelope envelope,
        QualityInspectionRun qualityRun,
        PcbExecutionAuditWindowProjection projection)
    {
        ArgumentNullException.ThrowIfNull(envelope);
        ArgumentNullException.ThrowIfNull(qualityRun);
        ArgumentNullException.ThrowIfNull(projection);

        var errors=new List<string>();
        if(!QualityInspectionRunValidationRuntime.IsValid(qualityRun))
            errors.Add("Quality inspection run is invalid.");

        if(projection.EvidenceEnvelopeFingerprint!=envelope.Fingerprint)
            errors.Add("Audit projection envelope fingerprint must match.");

        if(projection.QualityRunId!=qualityRun.RunId)
            errors.Add("Audit projection Quality run id must match.");

        var window=QualityInspectionAuditWindowRuntime.Create(qualityRun.Results);
        var expectedWindowFingerprint=
            PcbExecutionAuditWindowProjectionRuntime.CreateAuditWindowFingerprint(window);

        if(projection.AuditEnvelopeCount!=window.Count)
            errors.Add("Audit projection envelope count must match.");

        if(projection.AuditWindowFingerprint!=expectedWindowFingerprint)
            errors.Add("Audit projection window fingerprint must match.");

        if(projection.Fingerprint.Length!=64 ||
           !projection.Fingerprint.All(character=>
               Uri.IsHexDigit(character) &&
               char.ToLowerInvariant(character)==character))
        {
            errors.Add("Audit projection fingerprint must be 64 lowercase hexadecimal characters.");
        }

        if(errors.Count>0)
            return errors;

        var expected=PcbExecutionAuditWindowProjectionRuntime.CreateFingerprint(
            envelope.Fingerprint,
            qualityRun.RunId,
            expectedWindowFingerprint);

        if(expected!=projection.Fingerprint)
            errors.Add("Audit projection fingerprint does not match canonical content.");

        return errors;
    }

    public static bool IsValid(
        PcbProductionEvidenceEnvelope envelope,
        QualityInspectionRun qualityRun,
        PcbExecutionAuditWindowProjection projection)=>
        Validate(envelope,qualityRun,projection).Count==0;
}
