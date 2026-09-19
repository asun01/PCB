using System.Security.Cryptography;
using System.Text;
using Asun.Domain.Quality;
using Asun.Production.Runtime;

namespace Asun.Platform.ReplayIntegration;

public static class ProductionQualityEvidenceReplayBundleRuntime
{
    public static ProductionQualityEvidenceReplayBundle Create(
        ProductionSessionDefinition definition,
        ProductionSessionReport productionReport,
        QualityInspectionRun qualityRun,
        ProductionEvidenceReferenceProjection evidenceProjection)
    {
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(productionReport);
        ArgumentNullException.ThrowIfNull(qualityRun);
        ArgumentNullException.ThrowIfNull(evidenceProjection);

        if(!ProductionSessionValidationRuntime.IsValid(definition,productionReport))
            throw new ArgumentException("Production report is invalid.",nameof(productionReport));

        if(!QualityInspectionRunValidationRuntime.IsValid(qualityRun))
            throw new ArgumentException("Quality inspection run is invalid.",nameof(qualityRun));

        var qualityProjection=ProductionQualityInspectionProjectionRuntime.Create(
            productionReport,
            qualityRun);

        if(!ProductionQualityInspectionProjectionValidationRuntime.IsValid(
            productionReport,
            qualityRun,
            qualityProjection))
        {
            throw new ArgumentException(
                "Production-quality projection is invalid.",
                nameof(qualityRun));
        }

        if(!ProductionEvidenceReferenceProjectionValidationRuntime.IsValid(
            productionReport,
            evidenceProjection))
        {
            throw new ArgumentException("Evidence projection is invalid.",nameof(evidenceProjection));
        }

        if(productionReport.FrameCount!=qualityRun.ResultCount ||
           productionReport.FrameCount!=evidenceProjection.Frames.Count)
        {
            throw new ArgumentException("Production, Quality, and Evidence frame counts must align.");
        }

        var fingerprint=CreateFingerprint(
            productionReport.SessionId,
            productionReport.Fingerprint,
            qualityRun.RunId,
            evidenceProjection.Fingerprint,
            qualityRun.ResultCount,
            evidenceProjection.Frames.Count);

        return new ProductionQualityEvidenceReplayBundle(
            productionReport.SessionId,
            productionReport.Fingerprint,
            qualityRun.RunId,
            evidenceProjection.Fingerprint,
            qualityRun.ResultCount,
            evidenceProjection.Frames.Count,
            fingerprint);
    }

    internal static string CreateFingerprint(
        Guid productionSessionId,
        string productionFingerprint,
        Guid qualityRunId,
        string evidenceProjectionFingerprint,
        int qualityResultCount,
        int evidenceFrameCount)
    {
        var canonical=string.Join(
            "|",
            productionSessionId,
            productionFingerprint,
            qualityRunId,
            evidenceProjectionFingerprint,
            qualityResultCount,
            evidenceFrameCount);

        return Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(canonical)))
            .ToLowerInvariant();
    }
}
