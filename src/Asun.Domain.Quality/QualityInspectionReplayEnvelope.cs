namespace Asun.Domain.Quality;

public sealed record QualityInspectionReplayEnvelope(
    QualityInspectionReplayBundle Bundle,
    string BundleFingerprint);
