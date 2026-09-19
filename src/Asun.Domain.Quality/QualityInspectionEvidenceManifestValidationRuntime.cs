namespace Asun.Domain.Quality;

public static class QualityInspectionEvidenceManifestValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        QualityInspectionSnapshot snapshot,
        QualityInspectionEvidenceManifest manifest)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentNullException.ThrowIfNull(manifest);

        var errors = new List<string>();

        if (!QualityInspectionSnapshotValidationRuntime.IsValid(snapshot))
        {
            errors.Add("Inspection snapshot is invalid.");
            return errors;
        }

        errors.AddRange(
            QualityFindingEvidenceValidationRuntime.Validate(
                new QualityFindingEvidenceSet(manifest.Links)));

        var findingIds = snapshot.Findings.Findings
            .Select(finding => finding.Id)
            .ToHashSet();

        foreach (var link in manifest.Links)
        {
            if (!findingIds.Contains(link.FindingId))
                errors.Add("Evidence manifest contains an unknown finding.");
        }

        return errors;
    }

    public static bool IsValid(
        QualityInspectionSnapshot snapshot,
        QualityInspectionEvidenceManifest manifest) =>
        Validate(snapshot, manifest).Count == 0;
}
