using Asun.Metrology.Core;

namespace Asun.Domain.Pcb;

public sealed record CalibratedPcbPlacementObservation(
    PcbPlacementObservation Observation,
    MetrologyPoint2D SourceMeasuredPosition,
    string CalibrationFingerprint,
    string Fingerprint);
