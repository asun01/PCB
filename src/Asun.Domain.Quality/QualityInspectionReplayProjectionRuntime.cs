namespace Asun.Domain.Quality;

public static class QualityInspectionReplayProjectionRuntime
{
    public static QualityInspectionReplayProjection Create(
        QualityInspectionResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        if (!QualityInspectionResultValidationRuntime.IsValid(result))
            throw new ArgumentException(
                "Inspection result is invalid.",
                nameof(result));

        var findingIds = result.Findings.Findings
            .Select(finding => finding.Id)
            .OrderBy(id => id.Value, StringComparer.Ordinal)
            .ToArray();

        return new QualityInspectionReplayProjection(
            result.ResultId,
            result.SnapshotId,
            result.Sequence,
            findingIds,
            QualityInspectionEvidenceManifestRuntime.Create(result.Snapshot),
            QualityInspectionResultDeterminismRuntime
                .CreateContentFingerprint(result));
    }
}
