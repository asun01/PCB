using Asun.Domain.Pcb;
using Asun.Domain.Quality;
using Asun.Platform.MetrologyProductionIntegration;
using Asun.Platform.PipelineProductionIntegration;
using Asun.Production.Runtime;

namespace Asun.Platform.PcbExecutionIntegration;

public static class PcbExecutionSnapshotValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        PcbAssemblySnapshot assembly,
        ProductionSessionDefinition definition,
        ProductionSessionReport productionReport,
        IReadOnlyList<ProductionFrameProvenance> provenance,
        IReadOnlyList<ProductionMeasurementFact> measurementFacts,
        QualityInspectionRun qualityRun,
        ProductionEvidenceReferenceProjection evidenceProjection,
        ProductionPipelineReplayAudit pipelineAudit,
        PcbExecutionSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(productionReport);
        ArgumentNullException.ThrowIfNull(provenance);
        ArgumentNullException.ThrowIfNull(measurementFacts);
        ArgumentNullException.ThrowIfNull(qualityRun);
        ArgumentNullException.ThrowIfNull(evidenceProjection);
        ArgumentNullException.ThrowIfNull(pipelineAudit);
        ArgumentNullException.ThrowIfNull(snapshot);

        var errors=new List<string>();
        if(!PcbAssemblySnapshotValidationRuntime.IsValid(assembly))
            errors.Add("PCB assembly snapshot is invalid.");
        if(!ProductionSessionValidationRuntime.IsValid(definition,productionReport))
            errors.Add("Production session is invalid.");
        if(!ProductionFrameProvenanceRuntime.IsValid(productionReport,provenance))
            errors.Add("Production frame provenance is invalid.");
        if(!ProductionMeasurementFactValidationRuntime.IsValid(productionReport,measurementFacts))
            errors.Add("Production measurement facts are invalid.");
        if(!QualityInspectionRunValidationRuntime.IsValid(qualityRun))
            errors.Add("Quality inspection run is invalid.");
        if(!ProductionEvidenceReferenceProjectionValidationRuntime.IsValid(productionReport,evidenceProjection))
            errors.Add("Production evidence projection is invalid.");
        if(!ProductionPipelineReplayAuditValidationRuntime.IsValid(definition,productionReport,pipelineAudit))
            errors.Add("Production pipeline replay audit is invalid.");

        if(snapshot.AssemblyFingerprint!=assembly.Fingerprint)
            errors.Add("Execution snapshot assembly fingerprint must match.");
        if(snapshot.ProductionSessionId!=productionReport.SessionId)
            errors.Add("Execution snapshot production session id must match.");
        if(snapshot.ProductionFingerprint!=productionReport.Fingerprint)
            errors.Add("Execution snapshot production fingerprint must match.");
        if(snapshot.PipelineAuditFingerprint!=pipelineAudit.Fingerprint)
            errors.Add("Execution snapshot pipeline audit fingerprint must match.");
        if(snapshot.FrameCount!=productionReport.FrameCount)
            errors.Add("Execution snapshot frame count must match production.");
        if(snapshot.MeasurementFactCount!=measurementFacts.Count)
            errors.Add("Execution snapshot measurement fact count must match.");
        if(snapshot.QualityRunId!=qualityRun.RunId)
            errors.Add("Execution snapshot Quality run id must match.");
        if(snapshot.EvidenceProjectionFingerprint!=evidenceProjection.Fingerprint)
            errors.Add("Execution snapshot evidence projection fingerprint must match.");

        if(snapshot.Fingerprint.Length!=64 ||
           !snapshot.Fingerprint.All(character=>
               Uri.IsHexDigit(character) &&
               char.ToLowerInvariant(character)==character))
        {
            errors.Add("Execution snapshot fingerprint must be 64 lowercase hexadecimal characters.");
        }

        if(errors.Count>0)
            return errors;

        var expected=PcbExecutionSnapshotRuntime.CreateFingerprint(
            assembly,
            productionReport,
            provenance,
            measurementFacts,
            qualityRun,
            evidenceProjection,
            pipelineAudit);
        if(expected!=snapshot.Fingerprint)
            errors.Add("Execution snapshot fingerprint does not match canonical content.");

        return errors;
    }

    public static bool IsValid(
        PcbAssemblySnapshot assembly,
        ProductionSessionDefinition definition,
        ProductionSessionReport productionReport,
        IReadOnlyList<ProductionFrameProvenance> provenance,
        IReadOnlyList<ProductionMeasurementFact> measurementFacts,
        QualityInspectionRun qualityRun,
        ProductionEvidenceReferenceProjection evidenceProjection,
        ProductionPipelineReplayAudit pipelineAudit,
        PcbExecutionSnapshot snapshot)=>
        Validate(assembly,definition,productionReport,provenance,measurementFacts,qualityRun,evidenceProjection,pipelineAudit,snapshot).Count==0;
}
