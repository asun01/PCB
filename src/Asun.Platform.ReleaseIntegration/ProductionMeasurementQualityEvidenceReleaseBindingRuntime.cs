using System.Security.Cryptography;
using System.Text;
using Asun.Platform.QualityEvidenceIntegration;
using Asun.Release.Core;

namespace Asun.Platform.ReleaseIntegration;

public sealed record ProductionMeasurementQualityEvidenceReleaseBinding(
    long Sequence,
    Guid QualityResultId,
    string ComponentId,
    string EvidenceFingerprint,
    string ReleaseManifestFingerprint,
    bool ReleaseReady,
    string Fingerprint);

public static class ProductionMeasurementQualityEvidenceReleaseBindingRuntime
{
    public static ProductionMeasurementQualityEvidenceReleaseBinding Create(
        ProductionMeasurementQualityEvidenceBinding measurementEvidenceBinding,
        ReleaseManifest manifest)
    {
        ArgumentNullException.ThrowIfNull(measurementEvidenceBinding);
        ArgumentNullException.ThrowIfNull(manifest);

        if(measurementEvidenceBinding.Fingerprint.Length!=64)
            throw new ArgumentException("Measurement-quality-evidence binding is malformed.",nameof(measurementEvidenceBinding));
        if(!ReleaseManifestValidationRuntime.IsValid(manifest))
            throw new ArgumentException("Release manifest is invalid.",nameof(manifest));

        var readiness=ReleaseReadinessRuntime.Evaluate(manifest);
        var fingerprint=CreateFingerprint(
            measurementEvidenceBinding.Sequence,
            measurementEvidenceBinding.QualityResultId,
            measurementEvidenceBinding.ComponentId,
            measurementEvidenceBinding.EvidenceFingerprint,
            manifest.Fingerprint,
            readiness.Ready);

        return new ProductionMeasurementQualityEvidenceReleaseBinding(
            measurementEvidenceBinding.Sequence,
            measurementEvidenceBinding.QualityResultId,
            measurementEvidenceBinding.ComponentId,
            measurementEvidenceBinding.EvidenceFingerprint,
            manifest.Fingerprint,
            readiness.Ready,
            fingerprint);
    }

    public static IReadOnlyList<string> Validate(
        ProductionMeasurementQualityEvidenceBinding measurementEvidenceBinding,
        ReleaseManifest manifest,
        ProductionMeasurementQualityEvidenceReleaseBinding binding)
    {
        ArgumentNullException.ThrowIfNull(measurementEvidenceBinding);
        ArgumentNullException.ThrowIfNull(manifest);
        ArgumentNullException.ThrowIfNull(binding);

        var errors=new List<string>();
        if(measurementEvidenceBinding.Fingerprint.Length!=64)
            errors.Add("Measurement-quality-evidence binding fingerprint must be 64 characters.");
        if(!ReleaseManifestValidationRuntime.IsValid(manifest))
            errors.Add("Release manifest is invalid.");

        var readiness=ReleaseReadinessRuntime.Evaluate(manifest);
        if(binding.Sequence!=measurementEvidenceBinding.Sequence)
            errors.Add("Release binding sequence must match.");
        if(binding.QualityResultId!=measurementEvidenceBinding.QualityResultId)
            errors.Add("Release binding Quality result identity must match.");
        if(binding.ComponentId!=measurementEvidenceBinding.ComponentId)
            errors.Add("Release binding component identity must match.");
        if(binding.EvidenceFingerprint!=measurementEvidenceBinding.EvidenceFingerprint)
            errors.Add("Release binding Evidence fingerprint must match.");
        if(binding.ReleaseManifestFingerprint!=manifest.Fingerprint)
            errors.Add("Release binding manifest fingerprint must match.");
        if(binding.ReleaseReady!=readiness.Ready)
            errors.Add("Release binding readiness must match.");
        if(binding.Fingerprint.Length!=64 ||
           !binding.Fingerprint.All(c=>Uri.IsHexDigit(c) && char.ToLowerInvariant(c)==c))
            errors.Add("Release binding fingerprint must be 64 lowercase hexadecimal characters.");

        if(errors.Count==0)
        {
            var expected=CreateFingerprint(
                measurementEvidenceBinding.Sequence,
                measurementEvidenceBinding.QualityResultId,
                measurementEvidenceBinding.ComponentId,
                measurementEvidenceBinding.EvidenceFingerprint,
                manifest.Fingerprint,
                readiness.Ready);
            if(expected!=binding.Fingerprint)
                errors.Add("Release binding fingerprint does not match canonical content.");
        }

        return errors;
    }

    public static bool IsValid(
        ProductionMeasurementQualityEvidenceBinding measurementEvidenceBinding,
        ReleaseManifest manifest,
        ProductionMeasurementQualityEvidenceReleaseBinding binding)=>
        Validate(measurementEvidenceBinding,manifest,binding).Count==0;

    internal static string CreateFingerprint(
        long sequence,
        Guid qualityResultId,
        string componentId,
        string evidenceFingerprint,
        string manifestFingerprint,
        bool releaseReady)
    {
        var canonical=string.Join(
            "|",
            sequence,
            qualityResultId,
            componentId,
            evidenceFingerprint,
            manifestFingerprint,
            releaseReady);
        return Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(canonical))).ToLowerInvariant();
    }
}
