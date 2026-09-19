namespace Asun.Domain.Quality;

public sealed record QualityInspectionOutcomeSummary(
    int UnknownCount,
    int PassCount,
    int FailCount,
    int ReviewCount)
{
    public int TotalCount =>
        UnknownCount +
        PassCount +
        FailCount +
        ReviewCount;
}
