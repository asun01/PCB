using System.Security.Cryptography;
using System.Text;
using Asun.Domain.Quality;
using Asun.Platform.Evidence;
using Asun.Platform.QualityEvidenceIntegration;
using Asun.Platform.RoiProductionIntegration;

namespace Asun.Platform.RoiProductionIntegration;

public sealed record ProductionRoiQualityEvidenceReplayContext(
    Guid ProductionSessionId,
    string RoiContextBindingFingerprint,
    Guid QualityRunId,
    QualityFindingId FindingId,
    string EvidenceResolutionFingerprint,
    string DescriptorFingerprint,
    string BindingFingerprint);

public static class ProductionRoiQualityEvidenceReplayContextRuntime
{
    public static ProductionRoiQualityEvidenceReplayContext Create(
        ProductionRoiMeasurementQualityContext measurementQualityContext,
        QualityInspectionRun qualityRun,
        IReadOnlyList<QualityEvidenceHandleBinding> bindings,
        IReadOnlyList<QualityFindingEvidenceResolution> resolutions,
        IReadOnlyList<QualityFindingEvidenceReplayDescriptor> descriptors)
    {
        ArgumentNullException.ThrowIfNull(measurementQualityContext);
        ArgumentNullException.ThrowIfNull(qualityRun);
        ArgumentNullException.ThrowIfNull(bindings);
        ArgumentNullException.ThrowIfNull(resolutions);
        ArgumentNullException.ThrowIfNull(descriptors);

        var errors=Validate(
            measurementQualityContext,
            qualityRun,
            bindings,
            resolutions,
            descriptors);
        if(errors.Count>0)
            throw new ArgumentException(string.Join(" ",errors));

        var ordered=descriptors.OrderBy(item=>item.FindingId.Value,StringComparer.Ordinal).ToArray();
        var descriptor=ordered.Single();
        var canonical=string.Join("|",
            measurementQualityContext.ProductionSessionId,
            measurementQualityContext.RoiContextBindingFingerprint,
            qualityRun.RunId,
            descriptor.FindingId.Value,
            descriptor.ResolutionFingerprint,
            descriptor.DescriptorFingerprint);

        return new ProductionRoiQualityEvidenceReplayContext(
            measurementQualityContext.ProductionSessionId,
            measurementQualityContext.RoiContextBindingFingerprint,
            qualityRun.RunId,
            descriptor.FindingId,
            descriptor.ResolutionFingerprint,
            descriptor.DescriptorFingerprint,
            Hash(canonical));
    }

    public static IReadOnlyList<string> Validate(
        ProductionRoiMeasurementQualityContext measurementQualityContext,
        QualityInspectionRun qualityRun,
        IReadOnlyList<QualityEvidenceHandleBinding> bindings,
        IReadOnlyList<QualityFindingEvidenceResolution> resolutions,
        IReadOnlyList<QualityFindingEvidenceReplayDescriptor> descriptors)
    {
        ArgumentNullException.ThrowIfNull(measurementQualityContext);
        ArgumentNullException.ThrowIfNull(qualityRun);
        ArgumentNullException.ThrowIfNull(bindings);
        ArgumentNullException.ThrowIfNull(resolutions);
        ArgumentNullException.ThrowIfNull(descriptors);

        var errors=new List<string>();
        if(measurementQualityContext.ProductionSessionId==Guid.Empty)
            errors.Add("Measurement-quality Production session id cannot be empty.");
        if(measurementQualityContext.RoiContextBindingFingerprint.Length!=64 ||
           !IsLowerHex(measurementQualityContext.RoiContextBindingFingerprint))
            errors.Add("ROI context binding fingerprint is malformed.");

        if(!QualityInspectionRunValidationRuntime.IsValid(qualityRun))
            errors.Add("Quality inspection run is invalid.");

        errors.AddRange(
            QualityFindingEvidenceReplayDescriptorRuntime.Validate(
                qualityRun,
                bindings,
                resolutions,
                descriptors));

        if(descriptors.Count!=1)
            errors.Add("ROI quality evidence replay context requires exactly one finding descriptor.");

        if(bindings.Select(item=>item.EvidenceHandle).Distinct().Count()!=bindings.Count)
            errors.Add("Opaque evidence handles must be unique.");

        if(descriptors.Count==1)
        {
            var descriptor=descriptors[0];
            var result=qualityRun.Results.SingleOrDefault(item=>item.Findings.Findings.Any(
                finding=>finding.Id==descriptor.FindingId));

            if(result is null)
                errors.Add("Descriptor finding must belong to the Quality run.");
        }

        return errors;
    }

    public static bool IsValid(
        ProductionRoiMeasurementQualityContext measurementQualityContext,
        QualityInspectionRun qualityRun,
        IReadOnlyList<QualityEvidenceHandleBinding> bindings,
        IReadOnlyList<QualityFindingEvidenceResolution> resolutions,
        IReadOnlyList<QualityFindingEvidenceReplayDescriptor> descriptors)=>
        Validate(measurementQualityContext,qualityRun,bindings,resolutions,descriptors).Count==0;

    public static bool IsEquivalent(
        ProductionRoiQualityEvidenceReplayContext left,
        ProductionRoiQualityEvidenceReplayContext right)=>
        left.BindingFingerprint==right.BindingFingerprint;

    private static bool IsLowerHex(string value)=>
        value.All(c=>Uri.IsHexDigit(c) && char.ToLowerInvariant(c)==c);

    private static string Hash(string value)=>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant();
}
