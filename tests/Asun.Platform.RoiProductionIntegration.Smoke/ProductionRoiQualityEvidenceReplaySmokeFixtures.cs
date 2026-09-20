using Asun.Domain.Quality;
using Asun.Platform.Evidence;
using Asun.Platform.QualityEvidenceIntegration;
using Asun.Platform.RoiProductionIntegration;

namespace Asun.Platform.RoiProductionIntegration.Smoke;

internal static class ProductionRoiQualityEvidenceReplaySmokeFixtures
{
    public static (
        ProductionRoiMeasurementQualityContext MeasurementQualityContext,
        QualityInspectionRun QualityRun,
        IReadOnlyList<QualityEvidenceHandleBinding> Bindings,
        IReadOnlyList<QualityFindingEvidenceResolution> Resolutions,
        IReadOnlyList<QualityFindingEvidenceReplayDescriptor> Descriptors)
        Create()
    {
        var baseFixture=ProductionRoiMeasurementQualitySmokeFixtures.Create();
        var measurementQualityContext=
            ProductionRoiMeasurementQualityContextRuntime.Create(
                baseFixture.Production,
                baseFixture.RoiContext,
                baseFixture.Evaluation);

        var finding=baseFixture.Evaluation.Result.Findings.Findings.Single();
        var evidenceKey=QualityEvidenceKey.Create("roi-quality-evidence/1");
        var result=new QualityInspectionResult(
            baseFixture.Evaluation.Result.ResultId,
            new QualityInspectionSnapshot(
                baseFixture.Evaluation.Result.SnapshotId,
                baseFixture.Evaluation.Result.Sequence,
                new QualityFindingSet(baseFixture.Evaluation.Result.Findings.Findings),
                new QualityFindingEvidenceSet(new[]
                {
                    new QualityFindingEvidenceLink(finding.Id,evidenceKey)
                })));

        var qualityRun=QualityInspectionRunRuntime.Create(
            Guid.Parse("79000000-0000-0000-0000-000000000001"),
            new[]{result});

        var bindings=new[]
        {
            new QualityEvidenceHandleBinding(
                finding.Id,
                evidenceKey,
                EvidenceHandle.Create("roi-quality-evidence/opaque/1"))
        };

        var normalized=QualityEvidenceHandleBindingRuntime.Create(
            qualityRun,
            bindings);

        var resolutions=QualityFindingEvidenceResolutionRuntime.Create(
            qualityRun,
            normalized);

        var descriptors=QualityFindingEvidenceReplayDescriptorRuntime.Create(
            qualityRun,
            normalized,
            resolutions);

        return (
            measurementQualityContext,
            qualityRun,
            normalized,
            resolutions,
            descriptors);
    }
}
