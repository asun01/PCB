using System.Security.Cryptography;
using System.Text;
using Asun.Platform.ReleaseIntegration;

namespace Asun.Platform.ReplayIntegration;

public sealed record ProductionMeasurementQualityEvidenceReleaseReplayDescriptor(
    Guid ProductionSessionId,
    Guid QualityRunId,
    long Sequence,
    Guid QualityResultId,
    string ComponentId,
    string EvidenceFingerprint,
    string ReplayBundleFingerprint,
    string ReleaseManifestFingerprint,
    bool ReleaseReady,
    string ReleaseReplayBindingFingerprint,
    string DescriptorFingerprint);

public static class ProductionMeasurementQualityEvidenceReleaseReplayDescriptorRuntime
{
    public static ProductionMeasurementQualityEvidenceReleaseReplayDescriptor Create(
        ProductionMeasurementQualityEvidenceReleaseBinding measurementReleaseBinding,
        ProductionQualityEvidenceReleaseReplayBinding replayBinding)
    {
        ArgumentNullException.ThrowIfNull(measurementReleaseBinding);
        ArgumentNullException.ThrowIfNull(replayBinding);

        var errors=ValidateInputs(measurementReleaseBinding,replayBinding);
        if(errors.Count>0)
            throw new ArgumentException(string.Join(" ",errors),nameof(measurementReleaseBinding));

        var fingerprint=CreateFingerprint(
            replayBinding.ProductionSessionId,
            replayBinding.QualityRunId,
            measurementReleaseBinding.Sequence,
            measurementReleaseBinding.QualityResultId,
            measurementReleaseBinding.ComponentId,
            measurementReleaseBinding.EvidenceFingerprint,
            replayBinding.ReplayBundleFingerprint,
            measurementReleaseBinding.ReleaseManifestFingerprint,
            measurementReleaseBinding.ReleaseReady,
            replayBinding.Fingerprint);

        return new ProductionMeasurementQualityEvidenceReleaseReplayDescriptor(
            replayBinding.ProductionSessionId,
            replayBinding.QualityRunId,
            measurementReleaseBinding.Sequence,
            measurementReleaseBinding.QualityResultId,
            measurementReleaseBinding.ComponentId,
            measurementReleaseBinding.EvidenceFingerprint,
            replayBinding.ReplayBundleFingerprint,
            measurementReleaseBinding.ReleaseManifestFingerprint,
            measurementReleaseBinding.ReleaseReady,
            replayBinding.Fingerprint,
            fingerprint);
    }

    public static IReadOnlyList<string> Validate(
        ProductionMeasurementQualityEvidenceReleaseBinding measurementReleaseBinding,
        ProductionQualityEvidenceReleaseReplayBinding replayBinding,
        ProductionMeasurementQualityEvidenceReleaseReplayDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(measurementReleaseBinding);
        ArgumentNullException.ThrowIfNull(replayBinding);
        ArgumentNullException.ThrowIfNull(descriptor);

        var errors=ValidateInputs(measurementReleaseBinding,replayBinding);

        if(descriptor.ProductionSessionId!=replayBinding.ProductionSessionId)
            errors.Add("Replay descriptor production session identity must match.");
        if(descriptor.QualityRunId!=replayBinding.QualityRunId)
            errors.Add("Replay descriptor Quality run identity must match.");
        if(descriptor.Sequence!=measurementReleaseBinding.Sequence)
            errors.Add("Replay descriptor sequence must match.");
        if(descriptor.QualityResultId!=measurementReleaseBinding.QualityResultId)
            errors.Add("Replay descriptor Quality result identity must match.");
        if(descriptor.ComponentId!=measurementReleaseBinding.ComponentId)
            errors.Add("Replay descriptor component identity must match.");
        if(descriptor.EvidenceFingerprint!=measurementReleaseBinding.EvidenceFingerprint)
            errors.Add("Replay descriptor Evidence identity must match.");
        if(descriptor.ReplayBundleFingerprint!=replayBinding.ReplayBundleFingerprint)
            errors.Add("Replay descriptor replay bundle identity must match.");
        if(descriptor.ReleaseManifestFingerprint!=measurementReleaseBinding.ReleaseManifestFingerprint)
            errors.Add("Replay descriptor Release manifest identity must match.");
        if(descriptor.ReleaseReady!=measurementReleaseBinding.ReleaseReady)
            errors.Add("Replay descriptor Release readiness must match.");
        if(descriptor.ReleaseReplayBindingFingerprint!=replayBinding.Fingerprint)
            errors.Add("Replay descriptor Release replay binding identity must match.");

        if(descriptor.DescriptorFingerprint.Length!=64 || !IsLowerHex(descriptor.DescriptorFingerprint))
            errors.Add("Replay descriptor fingerprint must be 64 lowercase hexadecimal characters.");

        if(errors.Count==0)
        {
            var expected=CreateFingerprint(
                replayBinding.ProductionSessionId,
                replayBinding.QualityRunId,
                measurementReleaseBinding.Sequence,
                measurementReleaseBinding.QualityResultId,
                measurementReleaseBinding.ComponentId,
                measurementReleaseBinding.EvidenceFingerprint,
                replayBinding.ReplayBundleFingerprint,
                measurementReleaseBinding.ReleaseManifestFingerprint,
                measurementReleaseBinding.ReleaseReady,
                replayBinding.Fingerprint);
            if(expected!=descriptor.DescriptorFingerprint)
                errors.Add("Replay descriptor fingerprint does not match canonical content.");
        }

        return errors;
    }

    public static bool IsValid(
        ProductionMeasurementQualityEvidenceReleaseBinding measurementReleaseBinding,
        ProductionQualityEvidenceReleaseReplayBinding replayBinding,
        ProductionMeasurementQualityEvidenceReleaseReplayDescriptor descriptor)=>
        Validate(measurementReleaseBinding,replayBinding,descriptor).Count==0;

    internal static string CreateFingerprint(
        Guid productionSessionId,
        Guid qualityRunId,
        long sequence,
        Guid qualityResultId,
        string componentId,
        string evidenceFingerprint,
        string replayBundleFingerprint,
        string releaseManifestFingerprint,
        bool releaseReady,
        string releaseReplayBindingFingerprint)
    {
        var canonical=string.Join(
            "|",
            productionSessionId,
            qualityRunId,
            sequence,
            qualityResultId,
            componentId,
            evidenceFingerprint,
            replayBundleFingerprint,
            releaseManifestFingerprint,
            releaseReady,
            releaseReplayBindingFingerprint);
        return Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(canonical))).ToLowerInvariant();
    }

    private static List<string> ValidateInputs(
        ProductionMeasurementQualityEvidenceReleaseBinding measurementReleaseBinding,
        ProductionQualityEvidenceReleaseReplayBinding replayBinding)
    {
        var errors=new List<string>();

        if(measurementReleaseBinding.Fingerprint.Length!=64 || !IsLowerHex(measurementReleaseBinding.Fingerprint))
            errors.Add("Measurement release binding fingerprint is malformed.");
        if(replayBinding.ProductionSessionId==Guid.Empty)
            errors.Add("Replay binding production session identity is invalid.");
        if(replayBinding.QualityRunId==Guid.Empty)
            errors.Add("Replay binding Quality run identity is invalid.");
        if(measurementReleaseBinding.Sequence<=0)
            errors.Add("Measurement release binding sequence must be positive.");
        if(measurementReleaseBinding.QualityResultId==Guid.Empty)
            errors.Add("Measurement release binding Quality result identity is invalid.");
        if(string.IsNullOrWhiteSpace(measurementReleaseBinding.ComponentId))
            errors.Add("Measurement release binding component identity is required.");
        if(!IsLowerHex(measurementReleaseBinding.EvidenceFingerprint))
            errors.Add("Measurement release binding Evidence fingerprint must be 64 lowercase hexadecimal characters.");
        if(!IsLowerHex(replayBinding.ReplayBundleFingerprint))
            errors.Add("Replay binding replay bundle fingerprint must be 64 lowercase hexadecimal characters.");
        if(!IsLowerHex(replayBinding.ReleaseManifestFingerprint))
            errors.Add("Replay binding Release manifest fingerprint must be 64 lowercase hexadecimal characters.");
        if(replayBinding.ReleaseReady!=measurementReleaseBinding.ReleaseReady)
            errors.Add("Measurement and replay Release readiness must match.");
        if(replayBinding.ReleaseManifestFingerprint!=measurementReleaseBinding.ReleaseManifestFingerprint)
            errors.Add("Measurement and replay Release manifest identities must match.");
        if(!IsLowerHex(replayBinding.Fingerprint))
            errors.Add("Replay binding fingerprint must be 64 lowercase hexadecimal characters.");

        return errors;
    }

    private static bool IsLowerHex(string value)=>
        value.Length==64 && value.All(c=>Uri.IsHexDigit(c) && char.ToLowerInvariant(c)==c);
}
