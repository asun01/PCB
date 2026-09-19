using Asun.Metrology.Core;

namespace Asun.Domain.Pcb;

public static class PcbPlacementObservationRuntime
{
    public static PcbPlacementObservation Measure(
        PcbComponentReference component,
        MetrologyPoint2D measuredPosition)
    {
        ArgumentNullException.ThrowIfNull(component);

        if(!PcbComponentReferenceValidationRuntime.IsValid(component))
            throw new ArgumentException(
                "PCB component is invalid.",
                nameof(component));

        if(!measuredPosition.IsFinite)
            throw new ArgumentException(
                "Measured position must be finite.",
                nameof(measuredPosition));

        var delta=new MetrologyPoint2D(
            measuredPosition.X-component.Position.Xmm,
            measuredPosition.Y-component.Position.Ymm);

        var errorDistance=delta.DistanceTo(
            MetrologyPoint2D.Zero);

        return new PcbPlacementObservation(
            component.Id,
            component.Designator,
            new MetrologyPoint2D(
                component.Position.Xmm,
                component.Position.Ymm),
            measuredPosition,
            delta,
            errorDistance);
    }
}
