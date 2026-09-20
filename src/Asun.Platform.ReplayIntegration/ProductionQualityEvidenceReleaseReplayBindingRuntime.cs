using System.Security.Cryptography;
using System.Text;
using Asun.Release.Core;

namespace Asun.Platform.ReplayIntegration;

public sealed record ProductionQualityEvidenceReleaseReplayBinding(
    Guid ProductionSessionId,
    Guid QualityRunId,
    string ReplayBundleFingerprint,
    string ReleaseManifestFingerprint,
    bool ReleaseReady,
    string Fingerprint);

public static class ProductionQualityEvidenceReleaseReplayBindingRuntime
{
    public static ProductionQualityEvidenceReleaseReplayBinding Create(
        ProductionSessionDefinition definition,
        ProductionSessionReport productionReport,
        QualityInspectionRun qualityRun,
        ProductionEvidenceReferenceProjection evidenceProjection,
        ProductionQualityEvidenceReplayBundle bundle,
        ReleaseManifest manifest)
    {
        ArgumentNullException.ThrowIfNull(manifest);
        if(!ProductionQualityEvidenceReplayBundleValidationRuntime.IsValid(
            definition,productionReport,qualityRun,evidenceProjection,bundle))
            throw new ArgumentException("Replay bundle is invalid.",nameof(bundle));
        if(!ReleaseManifestValidationRuntime.IsValid(manifest))
            throw new ArgumentException("Release manifest is invalid.",nameof(manifest));

        var readiness=ReleaseReadinessRuntime.Evaluate(manifest);
        var fingerprint=CreateFingerprint(
            productionReport.SessionId,
            qualityRun.RunId,
            bundle.Fingerprint,
            manifest.Fingerprint,
            readiness.Ready);

        return new ProductionQualityEvidenceReleaseReplayBinding(
            productionReport.SessionId,
            qualityRun.RunId,
            bundle.Fingerprint,
            manifest.Fingerprint,
            readiness.Ready,
            fingerprint);
    }

    public static IReadOnlyList<string> Validate(
        ProductionSessionDefinition definition,
        ProductionSessionReport productionReport,
        QualityInspectionRun qualityRun,
        ProductionEvidenceReferenceProjection evidenceProjection,
        ProductionQualityEvidenceReplayBundle bundle,
        ReleaseManifest manifest,
        ProductionQualityEvidenceReleaseReplayBinding binding)
    {
        ArgumentNullException.ThrowIfNull(manifest);
        ArgumentNullException.ThrowIfNull(binding);

        var errors=new List<string>(
            ProductionQualityEvidenceReplayBundleValidationRuntime.Validate(
                definition,productionReport,qualityRun,evidenceProjection,bundle));

        if(!ReleaseManifestValidationRuntime.IsValid(manifest))
            errors.Add("Release manifest is invalid.");
        var readiness=ReleaseReadinessRuntime.Evaluate(manifest);

        if(binding.ProductionSessionId!=productionReport.SessionId)
            errors.Add("Release replay binding production session mismatch.");
        if(binding.QualityRunId!=qualityRun.RunId)
            errors.Add("Release replay binding Quality run mismatch.");
        if(binding.ReplayBundleFingerprint!=bundle.Fingerprint)
            errors.Add("Release replay binding bundle fingerprint mismatch.");
        if(binding.ReleaseManifestFingerprint!=manifest.Fingerprint)
            errors.Add("Release replay binding manifest fingerprint mismatch.");
        if(binding.ReleaseReady!=readiness.Ready)
            errors.Add("Release replay binding readiness mismatch.");
        if(binding.Fingerprint.Length!=64 ||
           !binding.Fingerprint.All(c=>Uri.IsHexDigit(c) && char.ToLowerInvariant(c)==c))
            errors.Add("Release replay binding fingerprint must be 64 lowercase hexadecimal characters.");

        if(errors.Count==0)
        {
            var expected=CreateFingerprint(
                productionReport.SessionId,
                qualityRun.RunId,
                bundle.Fingerprint,
                manifest.Fingerprint,
                readiness.Ready);
            if(expected!=binding.Fingerprint)
                errors.Add("Release replay binding fingerprint does not match canonical content.");
        }

        return errors;
    }

    public static bool IsValid(
        ProductionSessionDefinition definition,
        ProductionSessionReport productionReport,
        QualityInspectionRun qualityRun,
        ProductionEvidenceReferenceProjection evidenceProjection,
        ProductionQualityEvidenceReplayBundle bundle,
        ReleaseManifest manifest,
        ProductionQualityEvidenceReleaseReplayBinding binding)=>
        Validate(definition,productionReport,qualityRun,evidenceProjection,bundle,manifest,binding).Count==0;

    internal static string CreateFingerprint(
        Guid productionSessionId,
        Guid qualityRunId,
        string bundleFingerprint,
        string manifestFingerprint,
        bool releaseReady)
    {
        var canonical=string.Join("|",
            productionSessionId,
            qualityRunId,
            bundleFingerprint,
            manifestFingerprint,
            releaseReady);
        return Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(canonical))).ToLowerInvariant();
    }
}
