using System.Security.Cryptography;
using System.Text;
using Asun.Domain.Pcb;
using Asun.Domain.Quality;
using Asun.Platform.QualityEvidenceIntegration;
using Asun.Platform.MetrologyProductionIntegration;
using Asun.Platform.PipelineProductionIntegration;
using Asun.Production.Runtime;

namespace Asun.Platform.PcbExecutionIntegration;

public static class PcbExecutionSnapshotRuntime
{
    public static PcbExecutionSnapshot Create(
        PcbAssemblySnapshot assembly,
        ProductionSessionDefinition definition,
        ProductionSessionReport productionReport,
        IReadOnlyList<ProductionFrameProvenance> provenance,
        IReadOnlyList<ProductionMeasurementFact> measurementFacts,
        QualityInspectionRun qualityRun,
        ProductionEvidenceReferenceProjection evidenceProjection,
        ProductionPipelineReplayAudit pipelineAudit)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(productionReport);
        ArgumentNullException.ThrowIfNull(provenance);
        ArgumentNullException.ThrowIfNull(measurementFacts);
        ArgumentNullException.ThrowIfNull(qualityRun);
        ArgumentNullException.ThrowIfNull(evidenceProjection);
        ArgumentNullException.ThrowIfNull(pipelineAudit);

        if(!PcbAssemblySnapshotValidationRuntime.IsValid(assembly))
            throw new ArgumentException("PCB assembly snapshot is invalid.",nameof(assembly));
        if(!ProductionSessionValidationRuntime.IsValid(definition,productionReport))
            throw new ArgumentException("Production session is invalid.",nameof(productionReport));
        if(!ProductionFrameProvenanceRuntime.IsValid(productionReport,provenance))
            throw new ArgumentException("Production frame provenance is invalid.",nameof(provenance));
        if(!ProductionMeasurementFactValidationRuntime.IsValid(productionReport,measurementFacts))
            throw new ArgumentException("Production measurement facts are invalid.",nameof(measurementFacts));
        if(!QualityInspectionRunValidationRuntime.IsValid(qualityRun))
            throw new ArgumentException("Quality inspection run is invalid.",nameof(qualityRun));
        if(!ProductionEvidenceReferenceProjectionValidationRuntime.IsValid(productionReport,evidenceProjection))
            throw new ArgumentException("Production evidence projection is invalid.",nameof(evidenceProjection));
        if(!ProductionPipelineReplayAuditValidationRuntime.IsValid(definition,productionReport,pipelineAudit))
            throw new ArgumentException("Production pipeline replay audit is invalid.",nameof(pipelineAudit));

        if(productionReport.FrameCount!=measurementFacts.Count ||
           productionReport.FrameCount!=qualityRun.ResultCount ||
           productionReport.FrameCount!=evidenceProjection.Frames.Count)
        {
            throw new ArgumentException("Production, measurement, Quality, and evidence counts must align.");
        }

        var fingerprint=CreateFingerprint(
            assembly,
            productionReport,
            provenance,
            measurementFacts,
            qualityRun,
            evidenceProjection,
            pipelineAudit);

        return new PcbExecutionSnapshot(
            assembly.Fingerprint,
            productionReport.SessionId,
            productionReport.Fingerprint,
            pipelineAudit.Fingerprint,
            productionReport.FrameCount,
            measurementFacts.Count,
            qualityRun.RunId,
            evidenceProjection.Fingerprint,
            fingerprint);
    }

    internal static string CreateFingerprint(
        PcbAssemblySnapshot assembly,
        ProductionSessionReport productionReport,
        IReadOnlyList<ProductionFrameProvenance> provenance,
        IReadOnlyList<ProductionMeasurementFact> measurementFacts,
        QualityInspectionRun qualityRun,
        ProductionEvidenceReferenceProjection evidenceProjection,
        ProductionPipelineReplayAudit pipelineAudit)
    {
        var builder=new StringBuilder();
        builder.Append(assembly.Fingerprint).Append('|')
            .Append(productionReport.SessionId).Append('|')
            .Append(productionReport.Fingerprint).Append('|')
            .Append(pipelineAudit.Fingerprint).Append('|')
            .Append(qualityRun.RunId).Append('|')
            .Append(evidenceProjection.Fingerprint).Append('|');

        foreach(var frame in provenance.OrderBy(item=>item.Sequence.Value))
            builder.Append(frame.Sequence.Value).Append('|')
                .Append(frame.PayloadFingerprint).Append('|')
                .Append(frame.Width).Append('|')
                .Append(frame.Height).Append('|')
                .Append(frame.PixelFormat.Length).Append(':').Append(frame.PixelFormat).Append('|');

        foreach(var fact in measurementFacts.OrderBy(item=>item.Sequence))
            builder.Append(fact.Sequence).Append('|')
                .Append(fact.ProductionInputFingerprint).Append('|')
                .Append(fact.MeasuredPosition.X.ToString("R")).Append('|')
                .Append(fact.MeasuredPosition.Y.ToString("R")).Append('|')
                .Append(fact.ErrorDistance.ToString("R")).Append('|')
                .Append(fact.CalibrationFingerprint).Append('|')
                .Append(fact.ObservationFingerprint).Append('|');

        foreach(var result in qualityRun.Results.OrderBy(item=>item.Sequence).ThenBy(item=>item.ResultId))
            builder.Append(result.ResultId).Append('|')
                .Append(result.SnapshotId).Append('|')
                .Append(result.Sequence).Append('|')
                .Append(result.Findings.Count).Append('|')
                .Append(result.Evidence.Count).Append('|');

        return Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(builder.ToString())))
            .ToLowerInvariant();
    }
}
