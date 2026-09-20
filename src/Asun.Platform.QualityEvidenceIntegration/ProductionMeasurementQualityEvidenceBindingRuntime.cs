using System.Security.Cryptography;
using System.Text;
using Asun.Domain.Quality;
using Asun.Platform.Evidence;
using Asun.Platform.QualityIntegration;

namespace Asun.Platform.QualityEvidenceIntegration;

public sealed record ProductionMeasurementQualityEvidenceBinding(
    long Sequence,
    string ProductionInputFingerprint,
    Guid QualityResultId,
    QualityFindingId FindingId,
    string ComponentId,
    string CalibrationFingerprint,
    int EvidenceHandleCount,
    string EvidenceFingerprint,
    string Fingerprint);

public static class ProductionMeasurementQualityEvidenceBindingRuntime
{
    public static ProductionMeasurementQualityEvidenceBinding Create(
        ProductionMeasurementQualityBinding qualityBinding,
        QualityInspectionRun qualityRun,
        IReadOnlyList<QualityEvidenceHandleBinding> evidenceBindings)
    {
        ArgumentNullException.ThrowIfNull(qualityBinding);
        ArgumentNullException.ThrowIfNull(qualityRun);
        ArgumentNullException.ThrowIfNull(evidenceBindings);

        var errors=ValidateInputs(qualityBinding,qualityRun,evidenceBindings);
        if(errors.Count>0)
            throw new ArgumentException(string.Join(" ",errors),nameof(evidenceBindings));

        var result=qualityRun.Results.Single(item=>item.ResultId==qualityBinding.QualityResultId);
        var handles=evidenceBindings
            .Select(item=>item.EvidenceHandle)
            .OrderBy(handle=>handle.Value,StringComparer.Ordinal)
            .ToArray();
        var evidenceFingerprint=CreateEvidenceFingerprint(qualityBinding.QualityResultId,handles);
        var fingerprint=CreateFingerprint(
            qualityBinding.Sequence,
            qualityBinding.ProductionInputFingerprint,
            qualityBinding.QualityResultId,
            result.Findings.Findings[0].Id,
            qualityBinding.ComponentId.Value,
            qualityBinding.CalibrationFingerprint,
            handles.Length,
            evidenceFingerprint);

        return new ProductionMeasurementQualityEvidenceBinding(
            qualityBinding.Sequence,
            qualityBinding.ProductionInputFingerprint,
            qualityBinding.QualityResultId,
            result.Findings.Findings[0].Id,
            qualityBinding.ComponentId.Value,
            qualityBinding.CalibrationFingerprint,
            handles.Length,
            evidenceFingerprint,
            fingerprint);
    }

    public static IReadOnlyList<string> Validate(
        ProductionMeasurementQualityBinding qualityBinding,
        QualityInspectionRun qualityRun,
        IReadOnlyList<QualityEvidenceHandleBinding> evidenceBindings,
        ProductionMeasurementQualityEvidenceBinding binding)
    {
        ArgumentNullException.ThrowIfNull(binding);
        var errors=new List<string>(ValidateInputs(qualityBinding,qualityRun,evidenceBindings));

        var result=qualityRun.Results.SingleOrDefault(item=>item.ResultId==qualityBinding.QualityResultId);
        if(result is null)
        {
            errors.Add("Quality result identity is not present in the Quality run.");
            return errors;
        }

        var handles=evidenceBindings
            .Select(item=>item.EvidenceHandle)
            .OrderBy(handle=>handle.Value,StringComparer.Ordinal)
            .ToArray();
        var evidenceFingerprint=CreateEvidenceFingerprint(qualityBinding.QualityResultId,handles);

        if(binding.Sequence!=qualityBinding.Sequence)
            errors.Add("Measurement-quality-evidence sequence must match.");
        if(binding.ProductionInputFingerprint!=qualityBinding.ProductionInputFingerprint)
            errors.Add("Measurement-quality-evidence Production input fingerprint must match.");
        if(binding.QualityResultId!=qualityBinding.QualityResultId)
            errors.Add("Measurement-quality-evidence Quality result identity must match.");
        if(binding.FindingId!=result.Findings.Findings[0].Id)
            errors.Add("Measurement-quality-evidence finding identity must match.");
        if(binding.ComponentId!=qualityBinding.ComponentId.Value)
            errors.Add("Measurement-quality-evidence component identity must match.");
        if(binding.CalibrationFingerprint!=qualityBinding.CalibrationFingerprint)
            errors.Add("Measurement-quality-evidence calibration identity must match.");
        if(binding.EvidenceHandleCount!=handles.Length)
            errors.Add("Measurement-quality-evidence handle count must match.");
        if(binding.EvidenceFingerprint!=evidenceFingerprint)
            errors.Add("Measurement-quality-evidence fingerprint must match.");
        if(binding.Fingerprint.Length!=64 ||
           !binding.Fingerprint.All(c=>Uri.IsHexDigit(c) && char.ToLowerInvariant(c)==c))
            errors.Add("Measurement-quality-evidence binding fingerprint must be 64 lowercase hexadecimal characters.");

        if(errors.Count==0)
        {
            var expected=CreateFingerprint(
                qualityBinding.Sequence,
                qualityBinding.ProductionInputFingerprint,
                qualityBinding.QualityResultId,
                result.Findings.Findings[0].Id,
                qualityBinding.ComponentId.Value,
                qualityBinding.CalibrationFingerprint,
                handles.Length,
                evidenceFingerprint);
            if(expected!=binding.Fingerprint)
                errors.Add("Measurement-quality-evidence binding fingerprint does not match canonical content.");
        }

        return errors;
    }

    public static bool IsValid(
        ProductionMeasurementQualityBinding qualityBinding,
        QualityInspectionRun qualityRun,
        IReadOnlyList<QualityEvidenceHandleBinding> evidenceBindings,
        ProductionMeasurementQualityEvidenceBinding binding)=>
        Validate(qualityBinding,qualityRun,evidenceBindings,binding).Count==0;

    internal static IReadOnlyList<string> ValidateInputs(
        ProductionMeasurementQualityBinding qualityBinding,
        QualityInspectionRun qualityRun,
        IReadOnlyList<QualityEvidenceHandleBinding> evidenceBindings)
    {
        var errors=new List<string>();
        if(!QualityInspectionRunValidationRuntime.IsValid(qualityRun))
            errors.Add("Quality inspection run is invalid.");
        if(qualityRun.Results.All(item=>item.ResultId!=qualityBinding.QualityResultId))
            errors.Add("Measurement-quality Quality result is not present in the Quality run.");
        errors.AddRange(QualityEvidenceHandleBindingRuntime.Validate(qualityRun,evidenceBindings));
        if(string.IsNullOrWhiteSpace(qualityBinding.ProductionInputFingerprint))
            errors.Add("Production input fingerprint is required.");
        if(!qualityBinding.ComponentId.IsValid)
            errors.Add("Measurement-quality component identity is invalid.");
        if(qualityBinding.CalibrationFingerprint.Length!=64)
            errors.Add("Measurement-quality calibration fingerprint is invalid.");
        return errors;
    }

    internal static string CreateEvidenceFingerprint(
        Guid qualityResultId,
        IReadOnlyList<EvidenceHandle> handles)
    {
        var canonical=string.Join(
            "|",
            qualityResultId,
            string.Join(",",handles.OrderBy(handle=>handle.Value,StringComparer.Ordinal)
                .Select(handle=>handle.Value.Length+":"+handle.Value)));
        return Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(canonical))).ToLowerInvariant();
    }

    internal static string CreateFingerprint(
        long sequence,
        string productionInputFingerprint,
        Guid qualityResultId,
        QualityFindingId findingId,
        string componentId,
        string calibrationFingerprint,
        int evidenceHandleCount,
        string evidenceFingerprint)
    {
        var canonical=string.Join(
            "|",
            sequence,
            productionInputFingerprint,
            qualityResultId,
            findingId.Value,
            componentId,
            calibrationFingerprint,
            evidenceHandleCount,
            evidenceFingerprint);
        return Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(canonical))).ToLowerInvariant();
    }
}
