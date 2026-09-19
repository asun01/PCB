namespace Asun.Domain.Quality;

public sealed class QualityFindingEvidenceIndex
{
    private readonly IReadOnlyDictionary<
        QualityFindingId,
        IReadOnlyList<QualityEvidenceKey>> _byFinding;

    private readonly IReadOnlyDictionary<
        QualityEvidenceKey,
        IReadOnlyList<QualityFindingId>> _byEvidence;

    public QualityFindingEvidenceIndex(
        QualityFindingEvidenceSet set)
    {
        ArgumentNullException.ThrowIfNull(set);

        _byFinding = set.Links
            .GroupBy(link => link.FindingId)
            .ToDictionary(
                group => group.Key,
                group => (IReadOnlyList<QualityEvidenceKey>)group
                    .Select(link => link.EvidenceKey)
                    .Distinct()
                    .ToArray());

        _byEvidence = set.Links
            .GroupBy(link => link.EvidenceKey)
            .ToDictionary(
                group => group.Key,
                group => (IReadOnlyList<QualityFindingId>)group
                    .Select(link => link.FindingId)
                    .Distinct()
                    .ToArray());
    }

    public IReadOnlyList<QualityEvidenceKey> EvidenceFor(
        QualityFindingId findingId) =>
        _byFinding.TryGetValue(findingId, out var evidence)
            ? evidence
            : Array.Empty<QualityEvidenceKey>();

    public IReadOnlyList<QualityFindingId> FindingsFor(
        QualityEvidenceKey evidenceKey) =>
        _byEvidence.TryGetValue(evidenceKey, out var findings)
            ? findings
            : Array.Empty<QualityFindingId>();
}
