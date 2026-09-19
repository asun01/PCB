namespace Asun.Metrology.Core;

public static class DistanceMeasurementResultValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        DistanceMeasurementResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        var errors=new List<string>();

        errors.AddRange(
            MetrologyPoint2DValidationRuntime.Validate(result.Start));
        errors.AddRange(
            MetrologyPoint2DValidationRuntime.Validate(result.End));

        if(!double.IsFinite(result.Distance) || result.Distance<0)
            errors.Add("Measured distance must be finite and non-negative.");

        if(string.IsNullOrWhiteSpace(result.Unit))
        {
            errors.Add("Measurement unit cannot be blank.");
        }
        else if(result.ResultFingerprint.Length!=64 ||
           !result.ResultFingerprint.All(character=>
               Uri.IsHexDigit(character) &&
               char.ToLowerInvariant(character)==character))
        {
            errors.Add("Measurement result fingerprint must be 64 lowercase hexadecimal characters.");
        }

        if(string.IsNullOrWhiteSpace(result.Unit))
            return errors;

        var expected=DistanceMeasurementRuntime.Measure(
            result.Start,
            result.End,
            result.Unit);

        if(Math.Abs(expected.Distance-result.Distance)>1e-12)
            errors.Add("Measured distance does not match the source points.");

        if(expected.ResultFingerprint!=result.ResultFingerprint)
            errors.Add("Measurement result fingerprint does not match the result.");

        return errors;
    }

    public static bool IsValid(
        DistanceMeasurementResult result)=>
        Validate(result).Count==0;
}
