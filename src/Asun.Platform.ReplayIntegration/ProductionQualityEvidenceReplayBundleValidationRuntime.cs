using Asun.Domain.Quality;
using Asun.Production.Runtime;

namespace Asun.Platform.ReplayIntegration;

public static class ProductionQualityEvidenceReplayBundleValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        ProductionSessionDefinition definition,
        ProductionSessionReport productionReport,
        QualityInspectionRun qualityRun,
        ProductionEvidenceReferenceProjection evidenceProjection,
        ProductionQualityEvidenceReplayBundle bundle)
    {
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(productionReport);
        ArgumentNullException.ThrowIfNull(qualityRun);
        ArgumentNullException.ThrowIfNull(evidenceProjection);
        ArgumentNullException.ThrowIfNull(bundle);

        var errors=new List<string>();

        if(!ProductionSessionValidationRuntime.IsValid(definition,productionReport))
            errors.Add("Production report is invalid.");

        if(!QualityInspectionRunValidationRuntime.IsValid(qualityRun))
            errors.Add("Quality inspection run is invalid.");

        ProductionQualityInspectionProjection? qualityProjection=null;
        try
        {
            qualityProjection=ProductionQualityInspectionProjectionRuntime.Create(
                productionReport,
                qualityRun);
        }
        catch(ArgumentException exception)
        {
            errors.Add($"Production-quality projection cannot be reconstructed: {exception.Message}");
        }

        if(qualityProjection is not null &&
           !ProductionQualityInspectionProjectionValidationRuntime.IsValid(
               productionReport,
               qualityRun,
               qualityProjection))
        {
            errors.Add("Production-quality projection is invalid.");
        }

        if(!ProductionEvidenceReferenceProjectionValidationRuntime.IsValid(
            productionReport,
            evidenceProjection))
        {
            errors.Add("Evidence projection is invalid.");
        }

        if(bundle.ProductionSessionId!=productionReport.SessionId)
            errors.Add("Replay bundle production session id must match.");

        if(bundle.ProductionFingerprint!=productionReport.Fingerprint)
            errors.Add("Replay bundle production fingerprint must match.");

        if(bundle.QualityRunId!=qualityRun.RunId)
            errors.Add("Replay bundle quality run id must match.");

        if(bundle.EvidenceProjectionFingerprint!=evidenceProjection.Fingerprint)
            errors.Add("Replay bundle evidence projection fingerprint must match.");

        if(bundle.QualityResultCount!=qualityRun.ResultCount)
            errors.Add("Replay bundle quality result count must match.");

        if(bundle.EvidenceFrameCount!=evidenceProjection.Frames.Count)
            errors.Add("Replay bundle evidence frame count must match.");

        if(bundle.Fingerprint.Length!=64 ||
           !bundle.Fingerprint.All(character=>
               Uri.IsHexDigit(character) &&
               char.ToLowerInvariant(character)==character))
        {
            errors.Add("Replay bundle fingerprint must be 64 lowercase hexadecimal characters.");
        }

        if(errors.Count==0)
        {
            var expected=ProductionQualityEvidenceReplayBundleRuntime.CreateFingerprint(
                productionReport.SessionId,
                productionReport.Fingerprint,
                qualityRun.RunId,
                evidenceProjection.Fingerprint,
                qualityRun.ResultCount,
                evidenceProjection.Frames.Count);

            if(expected!=bundle.Fingerprint)
                errors.Add("Replay bundle fingerprint does not match its canonical content.");
        }

        return errors;
    }

    public static bool IsValid(
        ProductionSessionDefinition definition,
        ProductionSessionReport productionReport,
        QualityInspectionRun qualityRun,
        ProductionEvidenceReferenceProjection evidenceProjection,
        ProductionQualityEvidenceReplayBundle bundle)=>
        Validate(definition,productionReport,qualityRun,evidenceProjection,bundle).Count==0;
}
