namespace Asun.Domain.Quality;

public sealed class QualityInspectionFindingAuditIndex
{
    private readonly IReadOnlyDictionary<
        QualityFindingId,
        QualityInspectionFindingAuditRecord> _records;

    public QualityInspectionFindingAuditIndex(
        IEnumerable<QualityInspectionFindingAuditRecord> records)
    {
        ArgumentNullException.ThrowIfNull(records);

        _records = records.ToDictionary(
            record => record.FindingId);
    }

    public int Count => _records.Count;

    public QualityInspectionFindingAuditRecord? Find(
        QualityFindingId findingId) =>
        _records.TryGetValue(findingId, out var record)
            ? record
            : null;

    public IReadOnlyList<QualityFindingId> FindingIds =>
        _records.Keys
            .OrderBy(id => id.Value, StringComparer.Ordinal)
            .ToArray();
}
