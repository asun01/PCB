namespace Asun.Domain.Quality;

public sealed class QualityInspectionSnapshot
{
    public QualityInspectionSnapshot(
        Guid snapshotId,
        long sequence,
        QualityFindingSet findings,
        QualityFindingEvidenceSet evidence)
    {
        if (snapshotId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(snapshotId));

        if (sequence < 0)
            throw new ArgumentOutOfRangeException(nameof(sequence));

        ArgumentNullException.ThrowIfNull(findings);
        ArgumentNullException.ThrowIfNull(evidence);

        SnapshotId = snapshotId;
        Sequence = sequence;
        Findings = findings;
        Evidence = evidence;
    }

    public Guid SnapshotId { get; }

    public long Sequence { get; }

    public QualityFindingSet Findings { get; }

    public QualityFindingEvidenceSet Evidence { get; }
}
