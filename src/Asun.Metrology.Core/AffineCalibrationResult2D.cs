namespace Asun.Metrology.Core;

public sealed record AffineCalibrationResult2D(
    AffineTransform2D Transform,
    int PointCount,
    double RootMeanSquareError,
    double MaximumError,
    string Fingerprint);
