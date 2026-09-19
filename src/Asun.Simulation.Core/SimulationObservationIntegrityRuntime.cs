namespace Asun.Simulation.Core;

public static class SimulationObservationIntegrityRuntime
{
    public static IReadOnlyList<string> Validate(
        SimulatedBoardScenario scenario,
        SimulationObservation observation)
    {
        ArgumentNullException.ThrowIfNull(scenario);
        ArgumentNullException.ThrowIfNull(observation);

        var errors=new List<string>(
            SimulationObservationValidationRuntime.Validate(observation));

        if(errors.Count>0)
            return errors;

        var expected=SimulationObservationFingerprintRuntime.CreateFingerprint(
            scenario,
            observation);

        if(expected!=observation.Fingerprint)
            errors.Add("Simulation observation fingerprint does not match the scenario and observation content.");

        return errors;
    }

    public static bool IsValid(
        SimulatedBoardScenario scenario,
        SimulationObservation observation)=>
        Validate(scenario,observation).Count==0;
}
