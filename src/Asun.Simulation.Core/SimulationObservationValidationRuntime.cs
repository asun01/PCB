namespace Asun.Simulation.Core;

public static class SimulationObservationValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        SimulationObservation observation)
    {
        ArgumentNullException.ThrowIfNull(observation);

        var errors=new List<string>();

        if(!observation.Sequence.IsValid)
            errors.Add("Simulation observation sequence must be positive.");

        if(!observation.BoardOrigin.IsFinite)
            errors.Add("Simulation observation board origin must be finite.");

        if(observation.Defects.Any(defect=>!defect.IsValid))
            errors.Add("Simulation observation contains an invalid defect.");

        if(observation.Fingerprint.Length!=64 ||
           !observation.Fingerprint.All(character=>
               Uri.IsHexDigit(character) &&
               char.ToLowerInvariant(character)==character))
        {
            errors.Add("Simulation observation fingerprint must be 64 lowercase hexadecimal characters.");
        }

        return errors;
    }

    public static bool IsValid(
        SimulationObservation observation)=>
        Validate(observation).Count==0;
}
