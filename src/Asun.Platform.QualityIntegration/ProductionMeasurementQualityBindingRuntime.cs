using System.Security.Cryptography;
using System.Text;
using Asun.Domain.Pcb;
using Asun.Domain.Quality;
using Asun.Platform.MetrologyProductionIntegration;

namespace Asun.Platform.QualityIntegration;

public sealed record ProductionMeasurementQualityBinding(
    long Sequence,
    string ProductionInputFingerprint,
    PcbFeatureId ComponentId,
    string CalibrationFingerprint,
    string MeasurementObservationFingerprint,
    Guid QualityResultId,
    Guid QualitySnapshotId,
    string QualityEvaluationFingerprint,
    string Fingerprint);

public static class ProductionMeasurementQualityBindingRuntime
{
    public static ProductionMeasurementQualityBinding Create(
        ProductionMeasurementFact fact,
        CalibratedPcbPlacementObservation observation,
        ProductionMeasurementPcbBinding measurementBinding,
        PcbPlacementQualityEvaluation qualityEvaluation)
    {
        ArgumentNullException.ThrowIfNull(observation);
        ArgumentNullException.ThrowIfNull(measurementBinding);
        ArgumentNullException.ThrowIfNull(qualityEvaluation);

        var errors=ValidateInputs(fact,observation,measurementBinding,qualityEvaluation);
        if(errors.Count>0)
            throw new ArgumentException(string.Join(" ",errors),nameof(qualityEvaluation));

        var result=qualityEvaluation.Result;
        var fingerprint=CreateFingerprint(
            fact.Sequence,
            fact.ProductionInputFingerprint,
            observation.Observation.ComponentId,
            observation.CalibrationFingerprint,
            observation.Fingerprint,
            result.ResultId,
            result.SnapshotId,
            qualityEvaluation.Fingerprint);

        return new ProductionMeasurementQualityBinding(
            fact.Sequence,
            fact.ProductionInputFingerprint,
            observation.Observation.ComponentId,
            observation.CalibrationFingerprint,
            observation.Fingerprint,
            result.ResultId,
            result.SnapshotId,
            qualityEvaluation.Fingerprint,
            fingerprint);
    }

    public static IReadOnlyList<string> Validate(
        ProductionMeasurementFact fact,
        CalibratedPcbPlacementObservation observation,
        ProductionMeasurementPcbBinding measurementBinding,
        PcbPlacementQualityEvaluation qualityEvaluation,
        ProductionMeasurementQualityBinding binding)
    {
        ArgumentNullException.ThrowIfNull(binding);
        var errors=new List<string>(ValidateInputs(fact,observation,measurementBinding,qualityEvaluation));

        if(binding.Sequence!=fact.Sequence)
            errors.Add("Measurement-quality sequence must match.");
        if(binding.ProductionInputFingerprint!=fact.ProductionInputFingerprint)
            errors.Add("Measurement-quality Production input fingerprint must match.");
        if(binding.ComponentId!=observation.Observation.ComponentId)
            errors.Add("Measurement-quality component identity must match.");
        if(binding.CalibrationFingerprint!=observation.CalibrationFingerprint)
            errors.Add("Measurement-quality calibration fingerprint must match.");
        if(binding.MeasurementObservationFingerprint!=observation.Fingerprint)
            errors.Add("Measurement-quality observation fingerprint must match.");
        if(binding.QualityResultId!=qualityEvaluation.Result.ResultId)
            errors.Add("Measurement-quality Quality result identity must match.");
        if(binding.QualitySnapshotId!=qualityEvaluation.Result.SnapshotId)
            errors.Add("Measurement-quality Quality snapshot identity must match.");
        if(binding.QualityEvaluationFingerprint!=qualityEvaluation.Fingerprint)
            errors.Add("Measurement-quality evaluation fingerprint must match.");
        if(binding.Fingerprint.Length!=64 ||
           !binding.Fingerprint.All(c=>Uri.IsHexDigit(c) && char.ToLowerInvariant(c)==c))
            errors.Add("Measurement-quality binding fingerprint must be 64 lowercase hexadecimal characters.");

        if(errors.Count==0)
        {
            var expected=CreateFingerprint(
                fact.Sequence,
                fact.ProductionInputFingerprint,
                observation.Observation.ComponentId,
                observation.CalibrationFingerprint,
                observation.Fingerprint,
                qualityEvaluation.Result.ResultId,
                qualityEvaluation.Result.SnapshotId,
                qualityEvaluation.Fingerprint);
            if(expected!=binding.Fingerprint)
                errors.Add("Measurement-quality binding fingerprint does not match canonical content.");
        }

        return errors;
    }

    public static bool IsValid(
        ProductionMeasurementFact fact,
        CalibratedPcbPlacementObservation observation,
        ProductionMeasurementPcbBinding measurementBinding,
        PcbPlacementQualityEvaluation qualityEvaluation,
        ProductionMeasurementQualityBinding binding)=>
        Validate(fact,observation,measurementBinding,qualityEvaluation,binding).Count==0;

    internal static IReadOnlyList<string> ValidateInputs(
        ProductionMeasurementFact fact,
        CalibratedPcbPlacementObservation observation,
        ProductionMeasurementPcbBinding measurementBinding,
        PcbPlacementQualityEvaluation qualityEvaluation)
    {
        ArgumentNullException.ThrowIfNull(observation);
        ArgumentNullException.ThrowIfNull(measurementBinding);
        ArgumentNullException.ThrowIfNull(qualityEvaluation);

        var errors=new List<string>();
        errors.AddRange(ProductionMeasurementPcbBindingRuntime.Validate(fact,observation,measurementBinding));
        if(!PcbPlacementQualityEvaluationValidationRuntime.IsValid(qualityEvaluation))
            errors.Add("PCB placement Quality evaluation is invalid.");
        if(qualityEvaluation.Observation.ComponentId!=observation.Observation.ComponentId)
            errors.Add("Quality evaluation component identity must match calibrated observation.");
        if(qualityEvaluation.Result.Sequence!=fact.Sequence)
            errors.Add("Quality evaluation sequence must match measurement fact.");
        if(qualityEvaluation.Result.SnapshotId==Guid.Empty)
            errors.Add("Quality evaluation snapshot identity is invalid.");
        return errors;
    }

    internal static string CreateFingerprint(
        long sequence,
        string productionInputFingerprint,
        PcbFeatureId componentId,
        string calibrationFingerprint,
        string measurementObservationFingerprint,
        Guid qualityResultId,
        Guid qualitySnapshotId,
        string qualityEvaluationFingerprint)
    {
        var canonical=string.Join(
            "|",
            sequence,
            productionInputFingerprint,
            componentId.Value,
            calibrationFingerprint,
            measurementObservationFingerprint,
            qualityResultId,
            qualitySnapshotId,
            qualityEvaluationFingerprint);
        return Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(canonical))).ToLowerInvariant();
    }
}
