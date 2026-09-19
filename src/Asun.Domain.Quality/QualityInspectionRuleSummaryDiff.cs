namespace Asun.Domain.Quality;

public sealed record QualityInspectionRuleSummaryDiff(
    IReadOnlyList<string> AddedRuleCodes,
    IReadOnlyList<string> RemovedRuleCodes,
    IReadOnlyList<string> ChangedRuleCodes)
{
    public bool IsEmpty =>
        AddedRuleCodes.Count == 0 &&
        RemovedRuleCodes.Count == 0 &&
        ChangedRuleCodes.Count == 0;
}
