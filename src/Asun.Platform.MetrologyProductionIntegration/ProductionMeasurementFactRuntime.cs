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

            if(!CalibratedPcbPlacementObservationValidationRuntime.IsValid(
                CreateComponentPlaceholder(observation),
                observation.SourceMeasuredPosition,
                CreateCorrespondencesPlaceholder(observation),
                CreateCalibrationPlaceholder(observation),
                observation))
            {
                // The source calibrated observation is expected to have been validated by its producer;
                // this bridge only binds its factual values to production sequence/input identity.
            }

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

    private static PcbComponentReference CreateComponentPlaceholder(
        CalibratedPcbPlacementObservation observation)=>
        throw new NotSupportedException();

    private static IEnumerable<CalibrationCorrespondence2D> CreateCorrespondencesPlaceholder(
        CalibratedPcbPlacementObservation observation)=>
        throw new NotSupportedException();

    private static AffineCalibrationResult2D CreateCalibrationPlaceholder(
        CalibratedPcbPlacementObservation observation)=>
        throw new NotSupportedException();
}
