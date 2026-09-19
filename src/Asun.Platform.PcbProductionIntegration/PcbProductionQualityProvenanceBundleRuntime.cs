using System.Security.Cryptography;
using System.Text;
using Asun.Domain.Pcb;
using Asun.Domain.Quality;
using Asun.Production.Runtime;

namespace Asun.Platform.PcbProductionIntegration;

public static class PcbProductionQualityProvenanceBundleRuntime
{
    public static PcbProductionQualityProvenanceBundle Create(
        PcbAssemblySnapshot assembly,
        ProductionSessionDefinition definition,
        ProductionSessionReport productionReport,
        IReadOnlyList<ProductionFrameProvenance> provenance,
        QualityInspectionRun qualityRun)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(productionReport);
        ArgumentNullException.ThrowIfNull(provenance);
        ArgumentNullException.ThrowIfNull(qualityRun);

        if(!PcbAssemblySnapshotValidationRuntime.IsValid(assembly))
            throw new ArgumentException("PCB assembly snapshot is invalid.",nameof(assembly));

        if(!ProductionSessionValidationRuntime.IsValid(definition,productionReport))
            throw new ArgumentException("Production session is invalid.",nameof(productionReport));

        if(!ProductionFrameProvenanceRuntime.IsValid(productionReport,provenance))
            throw new ArgumentException("Production frame provenance is invalid.",nameof(provenance));

        if(!QualityInspectionRunValidationRuntime.IsValid(qualityRun))
            throw new ArgumentException("Quality inspection run is invalid.",nameof(qualityRun));

        if(productionReport.FrameCount!=qualityRun.ResultCount)
            throw new ArgumentException("Production frame count must match quality result count.");

        var fingerprint=CreateFingerprint(
            assembly,
            productionReport,
            provenance,
            qualityRun);

        return new PcbProductionQualityProvenanceBundle(
            assembly.Fingerprint,
            productionReport.SessionId,
            productionReport.Fingerprint,
            qualityRun.RunId,
            productionReport.FrameCount,
            assembly.Components.Count,
            fingerprint);
    }

    internal static string CreateFingerprint(
        PcbAssemblySnapshot assembly,
        ProductionSessionReport productionReport,
        IReadOnlyList<ProductionFrameProvenance> provenance,
        QualityInspectionRun qualityRun)
    {
        var builder=new StringBuilder();
        builder.Append(assembly.Fingerprint).Append('|')
            .Append(productionReport.SessionId).Append('|')
            .Append(productionReport.Fingerprint).Append('|')
            .Append(qualityRun.RunId).Append('|')
            .Append(productionReport.FrameCount).Append('|')
            .Append(assembly.Components.Count).Append('|');

        foreach(var frame in provenance.OrderBy(item=>item.Sequence.Value))
        {
            builder.Append(frame.Sequence.Value).Append('|')
                .Append(frame.PayloadFingerprint).Append('|')
                .Append(frame.Width).Append('|')
                .Append(frame.Height).Append('|')
                .Append(frame.PixelFormat.Length).Append(':').Append(frame.PixelFormat).Append('|')
                .Append(frame.CapturedAtUtc.UtcTicks).Append('|');
        }

        foreach(var result in qualityRun.Results.OrderBy(item=>item.Sequence).ThenBy(item=>item.SnapshotId))
        {
            builder.Append(result.ResultId).Append('|')
                .Append(result.SnapshotId).Append('|')
                .Append(result.Sequence).Append('|')
                .Append(result.Findings.Count).Append('|')
                .Append(result.Evidence.Count).Append('|');
        }

        return Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(builder.ToString())))
            .ToLowerInvariant();
    }
}
