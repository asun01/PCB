namespace Asun.Domain.Quality;

public sealed class QualityInspectionResult
{
    public QualityInspectionResult(
        Guid resultId,
        QualityInspectionSnapshot snapshot)
    {
        if (resultId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(resultId));

        ArgumentNullException.ThrowIfNull(snapshot);

        ResultId = resultId;
        Snapshot = snapshot;
    }

    public Guid ResultId { get; }

    public QualityInspectionSnapshot Snapshot { get; }

    public Guid SnapshotId => Snapshot.SnapshotId;

    public long Sequence => Snapshot.Sequence;

    public QualityFindingSet Findings => Snapshot.Findings;

    public QualityFindingEvidenceSet Evidence => Snapshot.Evidence;
}
