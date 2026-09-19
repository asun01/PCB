namespace Asun.Simulation.Core;

public static class SimulationSessionValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        IReadOnlyList<SimulationObservation> observations)
    {
        ArgumentNullException.ThrowIfNull(observations);

        var errors=new List<string>();
        long expected=1;

        foreach(var observation in observations)
        {
            errors.AddRange(
                SimulationObservationValidationRuntime.Validate(observation));

            if(observation.Sequence.Value!=expected)
                errors.Add("Simulation observation sequences must be contiguous from one.");

            expected++;
        }

        return errors;
    }

    public static bool IsValid(
        IReadOnlyList<SimulationObservation> observations)=>
        Validate(observations).Count==0;
}
