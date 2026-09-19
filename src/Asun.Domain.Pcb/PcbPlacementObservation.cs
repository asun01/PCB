using Asun.Metrology.Core;

namespace Asun.Domain.Pcb;

public sealed record PcbPlacementObservation(
    PcbFeatureId ComponentId,
    string Designator,
    MetrologyPoint2D ExpectedPosition,
    MetrologyPoint2D MeasuredPosition,
    MetrologyPoint2D Delta,
    double ErrorDistance);
