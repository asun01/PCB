using Asun.Metrology.Core;

namespace Asun.Domain.Pcb;

public sealed record PcbPlacementObservationSet(
    IReadOnlyList<PcbPlacementObservation> Observations,
    double MaximumError,
    double RootMeanSquareError);

public static class PcbPlacementObservationSetRuntime
{
    public static PcbPlacementObservationSet Create(
        IEnumerable<PcbPlacementObservation> observations)
    {
        ArgumentNullException.ThrowIfNull(observations);

        var materialized=observations.ToArray();

        if(materialized.Length==0)
            throw new ArgumentException(
                "Placement observation set cannot be empty.",
                nameof(observations));

        if(materialized.Any(observation=>
            !PcbPlacementObservationValidationRuntime.IsValid(observation)))
        {
            throw new ArgumentException(
                "Placement observation set contains invalid observations.",
                nameof(observations));
        }

        if(materialized.GroupBy(
                observation=>observation.ComponentId)
            .Any(group=>group.Count()>1))
        {
            throw new ArgumentException(
                "Placement observation component ids must be unique.",
                nameof(observations));
        }

        var ordered=materialized
            .OrderBy(observation=>observation.Designator,StringComparer.Ordinal)
            .ToArray();

        return new PcbPlacementObservationSet(
            ordered,
            ordered.Max(observation=>observation.ErrorDistance),
            Math.Sqrt(
                ordered
                    .Select(observation=>
                        observation.ErrorDistance*
                        observation.ErrorDistance)
                    .Average()));
    }
}
