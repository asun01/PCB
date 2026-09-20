using System.Security.Cryptography;
using System.Text;
using Asun.Platform.Evidence;
using Asun.Platform.PcbExecutionIntegration;
using Asun.Production.Runtime;

namespace Asun.Platform.PcbEvidenceResolutionIntegration;

public static class PcbExecutionEvidenceResolutionRuntime
{
    public static PcbExecutionEvidenceResolution Resolve(
        PcbExecutionSnapshot executionSnapshot,
        ProductionEvidenceReferenceProjection evidenceProjection,
        EvidenceCatalogSnapshot catalogSnapshot)
    {
        ArgumentNullException.ThrowIfNull(executionSnapshot);
        ArgumentNullException.ThrowIfNull(evidenceProjection);
        ArgumentNullException.ThrowIfNull(catalogSnapshot);

        if(!EvidenceCatalogSnapshotValidationRuntime.IsValid(catalogSnapshot))
            throw new ArgumentException("Evidence catalog snapshot is invalid.",nameof(catalogSnapshot));
        if(evidenceProjection.ProductionFingerprint!=executionSnapshot.ProductionFingerprint)
            throw new ArgumentException("Evidence projection must belong to the execution snapshot production report.",nameof(evidenceProjection));

        var requested=evidenceProjection.Frames
            .SelectMany(frame=>frame.Handles)
            .Distinct()
            .OrderBy(handle=>handle.Value,StringComparer.Ordinal)
            .ToArray();
        var resolution=EvidenceReferenceResolutionRuntime.Resolve(
            catalogSnapshot,
            new EvidenceReferenceSet(requested));
        var fingerprint=CreateFingerprint(
            executionSnapshot.Fingerprint,
            evidenceProjection.Fingerprint,
            resolution);

        return new PcbExecutionEvidenceResolution(
            executionSnapshot.Fingerprint,
            evidenceProjection.Fingerprint,
            resolution.SnapshotFingerprint,
            requested,
            resolution.FoundHandles,
            resolution.MissingHandles,
            fingerprint);
    }

    internal static string CreateFingerprint(
        string executionFingerprint,
        string evidenceProjectionFingerprint,
        EvidenceReferenceResolution resolution)
    {
        var builder=new StringBuilder();
        builder.Append(executionFingerprint).Append('|')
            .Append(evidenceProjectionFingerprint).Append('|')
            .Append(resolution.SnapshotFingerprint).Append('|');
        foreach(var handle in resolution.FoundHandles.OrderBy(handle=>handle.Value,StringComparer.Ordinal))
            builder.Append('F').Append(handle.Value.Length).Append(':').Append(handle.Value).Append('|');
        foreach(var handle in resolution.MissingHandles.OrderBy(handle=>handle.Value,StringComparer.Ordinal))
            builder.Append('M').Append(handle.Value.Length).Append(':').Append(handle.Value).Append('|');

        return Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(builder.ToString())))
            .ToLowerInvariant();
    }
}
