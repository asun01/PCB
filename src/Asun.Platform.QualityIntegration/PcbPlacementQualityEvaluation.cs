using Asun.Domain.Pcb;
using Asun.Domain.Quality;

namespace Asun.Platform.QualityIntegration;

public sealed record PcbPlacementQualityEvaluation(
    PcbPlacementObservation Observation,
    QualityInspectionResult Result,
    string Fingerprint);
