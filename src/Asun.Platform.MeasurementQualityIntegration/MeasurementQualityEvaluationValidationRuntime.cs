using Asun.Domain.Quality;
using Asun.Platform.MetrologyProductionIntegration;

namespace Asun.Platform.MeasurementQualityIntegration;

public static class MeasurementQualityEvaluationValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        MeasurementQualityEvaluation evaluation)
    {
        ArgumentNullException.ThrowIfNull(evaluation);

        var errors=new List<string>();
        if(!IsValidMeasurementFact(evaluation.Measurement))
            errors.Add("Measurement fact is invalid.");
        errors.AddRange(QualityInspectionResultValidationRuntime.Validate(evaluation.Result));

        if(evaluation.Result.Findings.Count!=1)
            errors.Add("Measurement quality evaluation must contain exactly one finding.");
        if(evaluation.Result.Evidence.Count!=0)
            errors.Add("Measurement quality evaluation must not fabricate evidence links.");
        if(evaluation.Fingerprint.Length!=64 ||
           !evaluation.Fingerprint.All(character=>
               Uri.IsHexDigit(character) &&
               char.ToLowerInvariant(character)==character))
        {
            errors.Add("Measurement quality evaluation fingerprint must be 64 lowercase hexadecimal characters.");
        }

        if(errors.Count>0)
            return errors;

        var expected=MeasurementQualityEvaluationRuntime.CreateFingerprint(
            evaluation.Measurement,
            evaluation.Result);
        if(expected!=evaluation.Fingerprint)
            errors.Add("Measurement quality evaluation fingerprint does not match canonical content.");

        return errors;
    }

    public static bool IsValid(
        MeasurementQualityEvaluation evaluation)=>
        Validate(evaluation).Count==0;

    private static bool IsValidMeasurementFact(ProductionMeasurementFact measurement)=>
        measurement.Sequence>0 &&
        measurement.SourceMeasuredPosition.IsFinite &&
        measurement.MeasuredPosition.IsFinite &&
        double.IsFinite(measurement.ErrorDistance) &&
        measurement.ErrorDistance>=0 &&
        measurement.ProductionInputFingerprint.Length==64 &&
        measurement.ProductionInputFingerprint.All(character=>
            Uri.IsHexDigit(character) &&
            char.ToLowerInvariant(character)==character) &&
        measurement.CalibrationFingerprint.Length==64 &&
        measurement.CalibrationFingerprint.All(character=>
            Uri.IsHexDigit(character) &&
            char.ToLowerInvariant(character)==character) &&
        measurement.ObservationFingerprint.Length==64 &&
        measurement.ObservationFingerprint.All(character=>
            Uri.IsHexDigit(character) &&
            char.ToLowerInvariant(character)==character);
}
