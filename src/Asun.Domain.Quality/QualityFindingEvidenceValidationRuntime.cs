namespace Asun.Domain.Quality;

public static class QualityFindingEvidenceValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        QualityFindingEvidenceLink link)
    {
        ArgumentNullException.ThrowIfNull(link);

        var errors = new List<string>();

        if (!link.FindingId.IsValid)
            errors.Add("Evidence link finding id must be valid.");

        if (!link.EvidenceKey.IsValid)
            errors.Add("Evidence link key must be valid.");

        return errors;
    }

    public static IReadOnlyList<string> Validate(
        QualityFindingEvidenceSet set)
    {
        ArgumentNullException.ThrowIfNull(set);

        var errors = new List<string>();
        var pairs = new HashSet<(QualityFindingId, QualityEvidenceKey)>();

        foreach (var link in set.Links)
        {
            if (!pairs.Add((link.FindingId, link.EvidenceKey)))
                errors.Add("Duplicate finding/evidence links are not allowed.");

            errors.AddRange(Validate(link));
        }

        return errors;
    }

    public static IReadOnlyList<string> Validate(
        QualityFindingSet findings,
        QualityFindingEvidenceSet links)
    {
        ArgumentNullException.ThrowIfNull(findings);
        ArgumentNullException.ThrowIfNull(links);

        var errors = new List<string>();
        var findingIds = findings.Findings
            .Select(finding => finding.Id)
            .ToHashSet();

        foreach (var link in links.Links)
        {
            if (!findingIds.Contains(link.FindingId))
                errors.Add("Evidence link points to an unknown finding.");
        }

        return errors;
    }

    public static bool IsValid(
        QualityFindingEvidenceLink link) =>
        Validate(link).Count == 0;

    public static bool IsValid(
        QualityFindingEvidenceSet set) =>
        Validate(set).Count == 0;

    public static bool IsValid(
        QualityFindingSet findings,
        QualityFindingEvidenceSet links) =>
        Validate(findings, links).Count == 0;
}
