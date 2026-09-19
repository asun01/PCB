using Asun.Domain.Pcb;
using Asun.Domain.Quality;

namespace Asun.Platform.QualityIntegration;

public static class PcbPlacementQualityEvaluationValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        PcbPlacementQualityEvaluation evaluation)
    {
        ArgumentNullException.ThrowIfNull(evaluation);

        var errors=new List<string>();
        errors.AddRange(PcbPlacementObservationValidationRuntime.Validate(evaluation.Observation));
        errors.AddRange(QualityInspectionResultValidationRuntime.Validate(evaluation.Result));

        if(evaluation.Result.Findings.Count!=1)
            errors.Add("Placement quality evaluation must contain exactly one finding.");

        if(evaluation.Result.Evidence.Count!=0)
            errors.Add("Placement quality evaluation does not invent evidence links.");

        if(evaluation.Result.Sequence<0)
            errors.Add("Placement quality evaluation sequence cannot be negative.");

        if(evaluation.Fingerprint.Length!=64 ||
           !evaluation.Fingerprint.All(character=>
               Uri.IsHexDigit(character) &&
               char.ToLowerInvariant(character)==character))
        {
            errors.Add("Placement quality evaluation fingerprint must be 64 lowercase hexadecimal characters.");
        }

        if(errors.Count==0)
        {
            var expected=PcbPlacementQualityEvaluationRuntime.CreateFingerprint(
                evaluation.Observation,
                evaluation.Result);

            if(expected!=evaluation.Fingerprint)
                errors.Add("Placement quality evaluation fingerprint does not match its canonical content.");
        }

        return errors;
    }

    public static bool IsValid(
        PcbPlacementQualityEvaluation evaluation)=>
        Validate(evaluation).Count==0;
}
