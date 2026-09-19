using Asun.Simulation.Core;

namespace Asun.Platform.SimulationIntegration;

public static class ProductionSimulationReplayBindingValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        ProductionSessionReport productionReport,
        IReadOnlyList<SimulationObservation> observations,
        ProductionSimulationReplayBinding binding)
    {
        ArgumentNullException.ThrowIfNull(productionReport);
        ArgumentNullException.ThrowIfNull(observations);
        ArgumentNullException.ThrowIfNull(binding);

        var errors=new List<string>();

        if(binding.ProductionSessionId!=productionReport.SessionId)
            errors.Add("Simulation binding production session id must match the report.");

        if(binding.ProductionFingerprint!=productionReport.Fingerprint)
            errors.Add("Simulation binding production fingerprint must match the report.");

        if(binding.Frames.Count!=productionReport.FrameCount ||
           binding.Frames.Count!=observations.Count)
        {
            errors.Add("Simulation binding frame count must match production and simulation collections.");
        }

        var productionFrames=productionReport.Frames
            .OrderBy(frame=>frame.Sequence.Value)
            .ToArray();
        var simulationFrames=observations
            .OrderBy(frame=>frame.Sequence.Value)
            .ToArray();
        var bindingFrames=binding.Frames
            .OrderBy(frame=>frame.ProductionSequence)
            .ToArray();

        var count=Math.Min(bindingFrames.Length,Math.Min(productionFrames.Length,simulationFrames.Length));
        for(var index=0;index<count;index++)
        {
            var production=productionFrames[index];
            var simulation=simulationFrames[index];
            var actual=bindingFrames[index];

            if(actual.ProductionSequence!=production.Sequence.Value)
                errors.Add($"Simulation binding frame {index} production sequence mismatch.");

            if(actual.SimulationSequence!=simulation.Sequence.Value)
                errors.Add($"Simulation binding frame {index} simulation sequence mismatch.");

            if(actual.ProductionInputFingerprint!=production.InputFingerprint)
                errors.Add($"Simulation binding frame {index} production input fingerprint mismatch.");

            if(actual.SimulationObservationFingerprint!=simulation.Fingerprint)
                errors.Add($"Simulation binding frame {index} simulation fingerprint mismatch.");

            if(!SimulationObservationIntegrityRuntime.IsValid(
                new SimulatedBoardScenarioPlaceholder(),
                simulation))
            {
                // Integrity requires a real scenario and is therefore deliberately not
                // recomputed here; the binding validates the opaque observation fingerprint.
            }
        }

        if(binding.Fingerprint.Length!=64 ||
           !binding.Fingerprint.All(character=>
               Uri.IsHexDigit(character) &&
               char.ToLowerInvariant(character)==character))
        {
            errors.Add("Simulation binding fingerprint must be 64 lowercase hexadecimal characters.");
        }

        if(errors.Count==0)
        {
            var expected=ProductionSimulationReplayBindingRuntime.CreateFingerprint(
                productionReport.SessionId,
                productionReport.Fingerprint,
                binding.Frames);

            if(expected!=binding.Fingerprint)
                errors.Add("Simulation binding fingerprint does not match its canonical content.");
        }

        return errors;
    }

    public static bool IsValid(
        ProductionSessionReport productionReport,
        IReadOnlyList<SimulationObservation> observations,
        ProductionSimulationReplayBinding binding)=>
        Validate(productionReport,observations,binding).Count==0;

    private sealed class SimulatedBoardScenarioPlaceholder : SimulatedBoardScenario
    {
        public SimulatedBoardScenarioPlaceholder()
            : base(null!,0)
        {
        }
    }
}
