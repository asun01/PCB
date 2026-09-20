using Asun.Platform.PcbExecutionIntegration;
using Asun.Platform.PcbEvidenceResolutionIntegration;
using Asun.Platform.PcbReleaseIntegration;
using Asun.Platform.PcbReplayIntegration;

namespace Asun.Platform.PcbEvidenceEnvelopeIntegration;

public static class PcbProductionEvidenceEnvelopeValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        PcbExecutionSnapshot executionSnapshot,
        PcbEndToEndReplaySnapshot replaySnapshot,
        PcbExecutionEvidenceResolution evidenceResolution,
        PcbExecutionReleaseProjection releaseProjection,
        PcbProductionEvidenceEnvelope envelope)
    {
        ArgumentNullException.ThrowIfNull(executionSnapshot);
        ArgumentNullException.ThrowIfNull(replaySnapshot);
        ArgumentNullException.ThrowIfNull(evidenceResolution);
        ArgumentNullException.ThrowIfNull(releaseProjection);
        ArgumentNullException.ThrowIfNull(envelope);

        var errors=new List<string>();
        if(replaySnapshot.ExecutionSnapshotFingerprint!=executionSnapshot.Fingerprint)
            errors.Add("Replay snapshot must belong to execution snapshot.");
        if(evidenceResolution.ExecutionSnapshotFingerprint!=executionSnapshot.Fingerprint)
            errors.Add("Evidence resolution must belong to execution snapshot.");
        if(releaseProjection.ExecutionSnapshotFingerprint!=executionSnapshot.Fingerprint)
            errors.Add("Release projection must belong to execution snapshot.");
        if(envelope.AssemblyFingerprint!=executionSnapshot.AssemblyFingerprint)
            errors.Add("Envelope assembly fingerprint must match execution snapshot.");
        if(envelope.ProductionSessionId!=executionSnapshot.ProductionSessionId)
            errors.Add("Envelope production session id must match execution snapshot.");
        if(envelope.ExecutionSnapshotFingerprint!=executionSnapshot.Fingerprint)
            errors.Add("Envelope execution fingerprint must match.");
        if(envelope.ReplaySnapshotFingerprint!=replaySnapshot.Fingerprint)
            errors.Add("Envelope replay fingerprint must match.");
        if(envelope.EvidenceResolutionFingerprint!=evidenceResolution.Fingerprint)
            errors.Add("Envelope evidence resolution fingerprint must match.");
        if(envelope.ReleaseProjectionFingerprint!=releaseProjection.Fingerprint)
            errors.Add("Envelope release projection fingerprint must match.");

        if(envelope.Fingerprint.Length!=64 ||
           !envelope.Fingerprint.All(character=>
               Uri.IsHexDigit(character) &&
               char.ToLowerInvariant(character)==character))
        {
            errors.Add("Envelope fingerprint must be 64 lowercase hexadecimal characters.");
        }

        if(errors.Count>0)
            return errors;

        var expected=PcbProductionEvidenceEnvelopeRuntime.CreateFingerprint(
            executionSnapshot,
            replaySnapshot,
            evidenceResolution,
            releaseProjection);
        if(expected!=envelope.Fingerprint)
            errors.Add("Envelope fingerprint does not match canonical content.");

        return errors;
    }

    public static bool IsValid(
        PcbExecutionSnapshot executionSnapshot,
        PcbEndToEndReplaySnapshot replaySnapshot,
        PcbExecutionEvidenceResolution evidenceResolution,
        PcbExecutionReleaseProjection releaseProjection,
        PcbProductionEvidenceEnvelope envelope)=>
        Validate(executionSnapshot,replaySnapshot,evidenceResolution,releaseProjection,envelope).Count==0;
}
