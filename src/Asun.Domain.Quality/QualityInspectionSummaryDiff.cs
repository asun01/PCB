namespace Asun.Domain.Quality;

public sealed record QualityInspectionSummaryDiff(
    bool OutcomeCountsChanged,
    bool SeverityCountsChanged,
    bool EvidenceCountsChanged,
    bool ContentFingerprintChanged)
{
    public bool IsEmpty =>
        !OutcomeCountsChanged &&
        !SeverityCountsChanged &&
        !EvidenceCountsChanged &&
        !ContentFingerprintChanged;
}
