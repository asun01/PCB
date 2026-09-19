using Asun.Metrology.Core;

namespace Asun.Domain.Pcb;

public static class PcbPlacementObservationValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        PcbPlacementObservation observation)
    {
        ArgumentNullException.ThrowIfNull(observation);

        var errors=new List<string>();

        if(!observation.ComponentId.IsValid)
            errors.Add("Placement observation component id must be valid.");

        if(string.IsNullOrWhiteSpace(observation.Designator))
            errors.Add("Placement observation designator cannot be blank.");

        if(!observation.ExpectedPosition.IsFinite ||
           !observation.MeasuredPosition.IsFinite ||
           !observation.Delta.IsFinite)
        {
            errors.Add("Placement observation positions must be finite.");
        }

        if(!double.IsFinite(observation.ErrorDistance) ||
           observation.ErrorDistance<0)
        {
            errors.Add("Placement observation error distance must be finite and non-negative.");
        }

        if(errors.Count>0)
            return errors;

        var expectedDelta=new MetrologyPoint2D(
            observation.MeasuredPosition.X-
                observation.ExpectedPosition.X,
            observation.MeasuredPosition.Y-
                observation.ExpectedPosition.Y);

        if(expectedDelta!=observation.Delta)
            errors.Add("Placement observation delta does not match expected and measured positions.");

        var expectedDistance=expectedDelta.DistanceTo(
            MetrologyPoint2D.Zero);

        if(Math.Abs(expectedDistance-observation.ErrorDistance)>1e-12)
            errors.Add("Placement observation error distance does not match the delta.");

        return errors;
    }

    public static bool IsValid(
        PcbPlacementObservation observation)=>
        Validate(observation).Count==0;
}
