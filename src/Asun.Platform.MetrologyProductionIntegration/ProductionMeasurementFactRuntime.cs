using Asun.Domain.Pcb;
using Asun.Metrology.Core;

namespace Asun.Platform.MetrologyProductionIntegration;

public static class ProductionMeasurementFactRuntime
{
    public static IReadOnlyList<ProductionMeasurementFact> Create(
        ProductionSessionReport productionReport,
        IReadOnlyList<long> sequences,
        IReadOnlyList<CalibratedPcbPlacementObservation> observations)
    {
        ArgumentNullException.ThrowIfNull(productionReport);
        ArgumentNullException.ThrowIfNull(sequences);
        ArgumentNullException.ThrowIfNull(observations);

        if(productionReport.FrameCount!=sequences.Count ||
           productionReport.FrameCount!=observations.Count)
        {
            throw new ArgumentException("Production, measurement sequence, and observation counts must match.");
        }

        var productionFrames=productionReport.Frames.OrderBy(frame=>frame.Sequence.Value).ToArray();
        var orderedSequences=sequences.OrderBy(sequence=>sequence).ToArray();
        var orderedObservations=observations.OrderBy(observation=>observation.Observation.ComponentId.Value,StringComparer.Ordinal).ToArray();
        var result=new List<ProductionMeasurementFact>(observations.Count);

        for(var index=0;index<productionFrames.Length;index++)
        {
            var production=productionFrames[index];
            var sequence=orderedSequences[index];
            var observation=orderedObservations[index];

            if(sequence!=production.Sequence.Value)
                throw new ArgumentException($"Measurement sequence {sequence} does not match production sequence {production.Sequence.Value}.");

            if(!IsValidObservationFact(observation))
                throw new ArgumentException(
                    $"Calibrated measurement observation {index} contains invalid factual values.",
                    nameof(observations));

            result.Add(
                new ProductionMeasurementFact(
                    production.Sequence.Value,
                    production.InputFingerprint,
                    observation.SourceMeasuredPosition,
                    observation.Observation.MeasuredPosition,
                    observation.Observation.ErrorDistance,
                    observation.CalibrationFingerprint,
                    observation.Fingerprint));
        }

        return result;
    }

    private static bool IsValidObservationFact(
        CalibratedPcbPlacementObservation observation)
    {
        var measured=observation.Observation;

        return observation.SourceMeasuredPosition.IsFinite &&
            measured.ExpectedPosition.IsFinite &&
            measured.MeasuredPosition.IsFinite &&
            measured.Delta.IsFinite &&
            double.IsFinite(measured.ErrorDistance) &&
            measured.ErrorDistance>=0 &&
            observation.CalibrationFingerprint.Length==64 &&
            observation.CalibrationFingerprint.All(character=>
                Uri.IsHexDigit(character) &&
                char.ToLowerInvariant(character)==character) &&
            observation.Fingerprint.Length==64 &&
            observation.Fingerprint.All(character=>
                Uri.IsHexDigit(character) &&
                char.ToLowerInvariant(character)==character);
    }
}
