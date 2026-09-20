using Asun.Domain.Quality;

namespace Asun.Platform.MeasurementQualityIntegration;

public sealed record MeasurementQualityEvaluation(
    ProductionMeasurementFact Measurement,
    QualityInspectionResult Result,
    string Fingerprint);
