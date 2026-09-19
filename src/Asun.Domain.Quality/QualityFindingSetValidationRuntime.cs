namespace Asun.Domain.Quality;

public static class QualityFindingSetValidationRuntime
{
    public static IReadOnlyList<string> Validate(QualityFindingSet set)
    {
        ArgumentNullException.ThrowIfNull(set);

        var errors = new List<string>();
        var ids = new HashSet<QualityFindingId>();

        foreach (var finding in set.Findings)
        {
            if (!ids.Add(finding.Id))
                errors.Add("Finding ids must be unique within a set.");

            errors.AddRange(QualityFindingValidationRuntime.Validate(finding));
        }

        return errors;
    }

    public static bool IsValid(QualityFindingSet set) =>
        Validate(set).Count == 0;
}
