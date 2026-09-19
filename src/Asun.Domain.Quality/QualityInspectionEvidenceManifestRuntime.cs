namespace Asun.Domain.Quality;

public static class QualityInspectionEvidenceManifestRuntime
{
    public static QualityInspectionEvidenceManifest Create(
        QualityInspectionSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        if (!QualityInspectionSnapshotValidationRuntime.IsValid(snapshot))
            throw new ArgumentException(
                "Inspection snapshot is invalid.",
                nameof(snapshot));

        return new QualityInspectionEvidenceManifest(
            snapshot.Evidence.Links
                .OrderBy(link => link.FindingId.Value, StringComparer.Ordinal)
                .ThenBy(link => link.EvidenceKey.Value, StringComparer.Ordinal)
                .ToArray());
    }
}
