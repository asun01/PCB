using Asun.Metrology.Core;

namespace Asun.Platform.MetrologyProductionIntegration;

public sealed record ProductionMeasurementFact(
    long Sequence,
    string ProductionInputFingerprint,
    MetrologyPoint2D SourceMeasuredPosition,
    MetrologyPoint2D MeasuredPosition,
    double ErrorDistance,
    string CalibrationFingerprint,
    string ObservationFingerprint);
