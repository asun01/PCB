namespace Asun.Simulation.Core;

public static class SimulationSessionRuntime
{
    public static IReadOnlyList<SimulationObservation> Run(
        SimulatedBoardScenario scenario,
        int frameCount)
    {
        ArgumentNullException.ThrowIfNull(scenario);

        if(frameCount<=0)
            throw new ArgumentOutOfRangeException(nameof(frameCount));

        var observations=new List<SimulationObservation>(frameCount);

        for(var sequence=1;sequence<=frameCount;sequence++)
        {
            var observation=SimulationScenarioRuntime.Observe(
                scenario,
                sequence);

            if(!SimulationObservationIntegrityRuntime.IsValid(scenario,observation))
                throw new InvalidOperationException("Simulation produced an invalid observation.");

            observations.Add(observation);
        }

        return observations;
    }
}
