namespace Asun.Domain.Quality;

/// <summary>
/// A vendor-neutral quality observation. It carries outcome/severity metadata but
/// does not define how a specific inspection rule computes acceptance.
/// </summary>
public sealed record QualityFinding(
    QualityFindingId Id,
    string RuleCode,
    QualityOutcome Outcome,
    QualitySeverity Severity,
    string Message)
{
    public bool IsValid =>
        Id.IsValid &&
        !string.IsNullOrWhiteSpace(RuleCode) &&
        Outcome != QualityOutcome.Unknown &&
        Severity is >= QualitySeverity.None and <= QualitySeverity.Critical &&
        !string.IsNullOrWhiteSpace(Message);
}
