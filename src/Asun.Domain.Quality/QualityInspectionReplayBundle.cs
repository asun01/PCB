namespace Asun.Domain.Quality;

public sealed record QualityInspectionReplayBundle(
    QualityInspectionReplayProjection? Previous,
    QualityInspectionReplayProjection Current,
    QualityInspectionReplayProjectionDiff Diff);
