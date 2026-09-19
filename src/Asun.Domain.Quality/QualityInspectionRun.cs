namespace Asun.Domain.Quality;

public sealed class QualityInspectionRun
{
    public QualityInspectionRun(
        Guid runId,
        IEnumerable<QualityInspectionResult> results)
    {
        if(runId==Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(runId));

        ArgumentNullException.ThrowIfNull(results);

        RunId=runId;
        Results=results.ToArray();
    }

    public Guid RunId { get; }

    public IReadOnlyList<QualityInspectionResult> Results { get; }

    public int ResultCount=>Results.Count;
}
