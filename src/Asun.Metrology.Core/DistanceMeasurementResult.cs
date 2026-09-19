namespace Asun.Metrology.Core;

public sealed record DistanceMeasurementResult(
    MetrologyPoint2D Start,
    MetrologyPoint2D End,
    double Distance,
    string Unit,
    string ResultFingerprint);
