namespace Asun.Domain.Quality;

public sealed record QualityInspectionSeveritySummary(
    int NoneCount,
    int InformationCount,
    int WarningCount,
    int ErrorCount,
    int CriticalCount)
{
    public int TotalCount =>
        NoneCount +
        InformationCount +
        WarningCount +
        ErrorCount +
        CriticalCount;
}
