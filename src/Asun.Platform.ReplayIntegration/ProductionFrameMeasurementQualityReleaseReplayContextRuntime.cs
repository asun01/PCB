using System.Security.Cryptography;
using System.Text;
using Asun.Platform.MeasurementQualityIntegration;

namespace Asun.Platform.ReplayIntegration;

public sealed record ProductionFrameMeasurementQualityReleaseReplayContext(
    long Sequence,
    string ProductionInputFingerprint,
    Guid QualityResultId,
    string EvidenceFingerprint,
    string ReleaseManifestFingerprint,
    bool ReleaseReady,
    string QualityEvaluationFingerprint,
    string ProvenanceBindingFingerprint,
    string DescriptorFingerprint,
    string BindingFingerprint);

public static class ProductionFrameMeasurementQualityReleaseReplayContextRuntime
{
    public static ProductionFrameMeasurementQualityReleaseReplayContext Create(
        ProductionFrameMeasurementQualityProvenanceBinding qualityProvenanceBinding,
        ProductionMeasurementQualityEvidenceReleaseReplayDescriptor replayDescriptor)
    {
        ArgumentNullException.ThrowIfNull(qualityProvenanceBinding);
        ArgumentNullException.ThrowIfNull(replayDescriptor);

        var errors=Validate(qualityProvenanceBinding,replayDescriptor);
        if(errors.Count>0)
            throw new ArgumentException(string.Join(" ",errors));

        var canonical=string.Join("|",
            qualityProvenanceBinding.Sequence,
            qualityProvenanceBinding.ProductionInputFingerprint,
            qualityProvenanceBinding.QualityResultId,
            replayDescriptor.EvidenceFingerprint,
            replayDescriptor.ReleaseManifestFingerprint,
            replayDescriptor.ReleaseReady,
            qualityProvenanceBinding.QualityEvaluationFingerprint,
            qualityProvenanceBinding.BindingFingerprint,
            replayDescriptor.DescriptorFingerprint);

        return new ProductionFrameMeasurementQualityReleaseReplayContext(
            qualityProvenanceBinding.Sequence,
            qualityProvenanceBinding.ProductionInputFingerprint,
            qualityProvenanceBinding.QualityResultId,
            replayDescriptor.EvidenceFingerprint,
            replayDescriptor.ReleaseManifestFingerprint,
            replayDescriptor.ReleaseReady,
            qualityProvenanceBinding.QualityEvaluationFingerprint,
            qualityProvenanceBinding.BindingFingerprint,
            replayDescriptor.DescriptorFingerprint,
            Hash(canonical));
    }

    public static IReadOnlyList<string> Validate(
        ProductionFrameMeasurementQualityProvenanceBinding qualityProvenanceBinding,
        ProductionMeasurementQualityEvidenceReleaseReplayDescriptor replayDescriptor)
    {
        ArgumentNullException.ThrowIfNull(qualityProvenanceBinding);
        ArgumentNullException.ThrowIfNull(replayDescriptor);

        var errors=new List<string>();

        if(qualityProvenanceBinding.Sequence<=0)
            errors.Add("Frame/measurement/quality sequence must be positive.");
        if(!IsLowerHex(qualityProvenanceBinding.ProductionInputFingerprint))
            errors.Add("Frame/measurement/quality Production input fingerprint is malformed.");
        if(qualityProvenanceBinding.QualityResultId==Guid.Empty)
            errors.Add("Frame/measurement/quality Quality result identity cannot be empty.");
        if(!IsLowerHex(qualityProvenanceBinding.QualityEvaluationFingerprint))
            errors.Add("Quality evaluation fingerprint is malformed.");
        if(!IsLowerHex(qualityProvenanceBinding.BindingFingerprint))
            errors.Add("Frame/measurement/quality provenance binding fingerprint is malformed.");

        if(replayDescriptor.Sequence!=qualityProvenanceBinding.Sequence)
            errors.Add("Replay descriptor sequence must match frame/measurement/quality provenance.");
        if(replayDescriptor.ProductionInputFingerprint!=qualityProvenanceBinding.ProductionInputFingerprint)
            errors.Add("Replay descriptor Production input fingerprint must match.");
        if(replayDescriptor.QualityResultId!=qualityProvenanceBinding.QualityResultId)
            errors.Add("Replay descriptor Quality result identity must match.");
        if(!IsLowerHex(replayDescriptor.EvidenceFingerprint))
            errors.Add("Replay descriptor Evidence fingerprint is malformed.");
        if(!IsLowerHex(replayDescriptor.ReleaseManifestFingerprint))
            errors.Add("Replay descriptor Release manifest fingerprint is malformed.");
        if(!IsLowerHex(replayDescriptor.DescriptorFingerprint))
            errors.Add("Replay descriptor fingerprint is malformed.");
        if(replayDescriptor.ProductionSessionId==Guid.Empty || replayDescriptor.QualityRunId==Guid.Empty)
            errors.Add("Replay descriptor Production/Quality identity cannot be empty.");

        return errors;
    }

    public static bool IsValid(
        ProductionFrameMeasurementQualityProvenanceBinding qualityProvenanceBinding,
        ProductionMeasurementQualityEvidenceReleaseReplayDescriptor replayDescriptor)=>
        Validate(qualityProvenanceBinding,replayDescriptor).Count==0;

    public static bool IsEquivalent(
        ProductionFrameMeasurementQualityReleaseReplayContext left,
        ProductionFrameMeasurementQualityReleaseReplayContext right)=>
        left.BindingFingerprint==right.BindingFingerprint;

    private static bool IsLowerHex(string value)=>
        value.Length==64 && value.All(c=>Uri.IsHexDigit(c) && char.ToLowerInvariant(c)==c);

    private static string Hash(string value)=>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant();
}
