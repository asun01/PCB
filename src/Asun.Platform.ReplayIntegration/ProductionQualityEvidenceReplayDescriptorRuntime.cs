using System.Security.Cryptography;
using System.Text;
using Asun.Domain.Quality;
using Asun.Production.Runtime;

namespace Asun.Platform.ReplayIntegration;

public sealed record ProductionQualityEvidenceReplayDescriptor(
    Guid ProductionSessionId,
    Guid QualityRunId,
    string ProductionFingerprint,
    string EvidenceProjectionFingerprint,
    int QualityResultCount,
    int EvidenceFrameCount,
    string BundleFingerprint,
    string DescriptorFingerprint);

public static class ProductionQualityEvidenceReplayDescriptorRuntime
{
    public static ProductionQualityEvidenceReplayDescriptor Create(
        ProductionSessionDefinition definition,
        ProductionSessionReport productionReport,
        QualityInspectionRun qualityRun,
        ProductionEvidenceReferenceProjection evidenceProjection,
        ProductionQualityEvidenceReplayBundle bundle)
    {
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(productionReport);
        ArgumentNullException.ThrowIfNull(qualityRun);
        ArgumentNullException.ThrowIfNull(evidenceProjection);
        ArgumentNullException.ThrowIfNull(bundle);

        if(!ProductionQualityEvidenceReplayBundleValidationRuntime.IsValid(
            definition,productionReport,qualityRun,evidenceProjection,bundle))
            throw new ArgumentException("Production quality evidence replay bundle is invalid.",nameof(bundle));

        var canonical=string.Join(
            "|",
            bundle.ProductionSessionId,
            bundle.QualityRunId,
            bundle.ProductionFingerprint,
            bundle.EvidenceProjectionFingerprint,
            bundle.QualityResultCount,
            bundle.EvidenceFrameCount,
            bundle.Fingerprint);

        var fingerprint=Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(canonical))).ToLowerInvariant();

        return new ProductionQualityEvidenceReplayDescriptor(
            bundle.ProductionSessionId,
            bundle.QualityRunId,
            bundle.ProductionFingerprint,
            bundle.EvidenceProjectionFingerprint,
            bundle.QualityResultCount,
            bundle.EvidenceFrameCount,
            bundle.Fingerprint,
            fingerprint);
    }

    public static IReadOnlyList<string> Validate(
        ProductionSessionDefinition definition,
        ProductionSessionReport productionReport,
        QualityInspectionRun qualityRun,
        ProductionEvidenceReferenceProjection evidenceProjection,
        ProductionQualityEvidenceReplayBundle bundle,
        ProductionQualityEvidenceReplayDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(productionReport);
        ArgumentNullException.ThrowIfNull(qualityRun);
        ArgumentNullException.ThrowIfNull(evidenceProjection);
        ArgumentNullException.ThrowIfNull(bundle);
        ArgumentNullException.ThrowIfNull(descriptor);

        var errors=new List<string>(
            ProductionQualityEvidenceReplayBundleValidationRuntime.Validate(
                definition,productionReport,qualityRun,evidenceProjection,bundle));

        if(descriptor.ProductionSessionId!=bundle.ProductionSessionId)
            errors.Add("Replay descriptor production session identity must match.");
        if(descriptor.QualityRunId!=bundle.QualityRunId)
            errors.Add("Replay descriptor Quality run identity must match.");
        if(descriptor.ProductionFingerprint!=bundle.ProductionFingerprint)
            errors.Add("Replay descriptor production fingerprint must match.");
        if(descriptor.EvidenceProjectionFingerprint!=bundle.EvidenceProjectionFingerprint)
            errors.Add("Replay descriptor evidence fingerprint must match.");
        if(descriptor.QualityResultCount!=bundle.QualityResultCount)
            errors.Add("Replay descriptor Quality result count must match.");
        if(descriptor.EvidenceFrameCount!=bundle.EvidenceFrameCount)
            errors.Add("Replay descriptor evidence frame count must match.");
        if(descriptor.BundleFingerprint!=bundle.Fingerprint)
            errors.Add("Replay descriptor bundle fingerprint must match.");
        if(descriptor.DescriptorFingerprint.Length!=64 ||
           !descriptor.DescriptorFingerprint.All(character=>Uri.IsHexDigit(character) &&
               char.ToLowerInvariant(character)==character))
            errors.Add("Replay descriptor fingerprint must be 64 lowercase hexadecimal characters.");

        if(errors.Count>0)
            return errors;

        var expected=Create(definition,productionReport,qualityRun,evidenceProjection,bundle);
        if(expected.DescriptorFingerprint!=descriptor.DescriptorFingerprint)
            errors.Add("Replay descriptor fingerprint does not match canonical content.");

        return errors;
    }

    public static bool IsValid(
        ProductionSessionDefinition definition,
        ProductionSessionReport productionReport,
        QualityInspectionRun qualityRun,
        ProductionEvidenceReferenceProjection evidenceProjection,
        ProductionQualityEvidenceReplayBundle bundle,
        ProductionQualityEvidenceReplayDescriptor descriptor)=>
        Validate(definition,productionReport,qualityRun,evidenceProjection,bundle,descriptor).Count==0;

    public static bool IsEquivalent(
        ProductionQualityEvidenceReplayDescriptor left,
        ProductionQualityEvidenceReplayDescriptor right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);
        return left.DescriptorFingerprint==right.DescriptorFingerprint;
    }
}
